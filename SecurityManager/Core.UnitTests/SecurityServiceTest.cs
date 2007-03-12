using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using log4net;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.UnitTests.Domain;
using Rubicon.SecurityManager.UnitTests.TestDomain;
using Mocks_Is = Rhino.Mocks.Is;
using Mocks_List = Rhino.Mocks.List;
using Mocks_Property = Rhino.Mocks.Property;

namespace Rubicon.SecurityManager.UnitTests
{
  [TestFixture]
  public class SecurityServiceTest : DomainTest
  {
    private MockRepository _mocks;
    private IAccessControlListFinder _mockAclFinder;
    private ISecurityTokenBuilder _mockTokenBuilder;

    private SecurityService _service;
    private SecurityContext _context;
    private ClientTransaction _transaction;
    private AccessControlEntry _ace;
    private IPrincipal _principal;

    private MemoryAppender _memoryAppender;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

     _mocks = new MockRepository ();
     _mockAclFinder = _mocks.CreateMock<IAccessControlListFinder> ();
     _mockTokenBuilder = _mocks.CreateMock<ISecurityTokenBuilder> ();

     _service = new SecurityService (_mockAclFinder, _mockTokenBuilder);
     _context = new SecurityContext (typeof (Order), "Owner", "UID: OwnerGroup", "OwnerClient", null, null);
     _transaction = new ClientTransaction ();
     _ace = CreateAce (_transaction);
     _principal = CreateUser ();
  
      _memoryAppender = new MemoryAppender();
      BasicConfigurator.Configure(_memoryAppender); 
    }

    [TearDown]
    public void TearDown()
    {
      LogManager.ResetConfiguration();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      SecurityService provider = new SecurityService ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetAccess_WithoutAccess ()
    {
      SecurityToken token = new SecurityToken (null, new List<Group> (), new List<AbstractRoleDefinition> ());
      
      Expect.Call (_mockAclFinder.Find (_transaction, _context)).Return (CreateAcl (_transaction, _ace));
      Expect.Call (_mockTokenBuilder.CreateToken (_transaction, _principal, _context)).Return (token);

      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_transaction, _context, _principal);

      _mocks.VerifyAll ();
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAccess_WithReadAccess ()
    {
      List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition> ();
      roles.Add (_ace.SpecificAbstractRole);
      SecurityToken token = new SecurityToken (null, new List<Group> (), roles);

      Expect.Call (_mockAclFinder.Find (_transaction, _context)).Return (CreateAcl (_transaction, _ace));
      Expect.Call (_mockTokenBuilder.CreateToken (_transaction, _principal, _context)).Return(token);

      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_transaction, _context, _principal);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (EnumWrapper.Parse ("Read|MyTypeName")), accessTypes);
    }

    [Test]
    public void GetAccess_WithReadAccessFromInterface ()
    {
      List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition> ();
      roles.Add (_ace.SpecificAbstractRole);
      SecurityToken token = new SecurityToken (null, new List<Group> (), roles);

      Expect.Call (_mockAclFinder.Find (null, null)).Return (CreateAcl (_transaction, _ace)).Constraints (
          Mocks_Is.NotNull (),
          Mocks_Is.Same (_context));
      Expect.Call (_mockTokenBuilder.CreateToken (null, null, null)).Return (token).Constraints (
          Mocks_Is.NotNull (),
          Mocks_Is.Same (_principal),
          Mocks_Is.Same (_context));

      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_context, _principal);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (EnumWrapper.Parse ("Read|MyTypeName")), accessTypes);
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromAccessControlListFinder ()
    {
      SecurityToken token = new SecurityToken (null, new List<Group> (), new List<AbstractRoleDefinition> ());
      AccessControlException expectedException = new AccessControlException ();

      Expect.Call (_mockAclFinder.Find (_transaction, _context)).Throw (expectedException);

      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_transaction, _context, _principal);

      _mocks.VerifyAll ();
      Assert.AreEqual (0, accessTypes.Length);
      log4net.Core.LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreSame (expectedException, events[0].ExceptionObject);
      Assert.AreEqual (log4net.Core.Level.Error, events[0].Level);
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromSecurityTokenBuilder ()
    {
      SecurityToken token = new SecurityToken (null, new List<Group> (), new List<AbstractRoleDefinition> ());
      AccessControlException expectedException = new AccessControlException ();

      Expect.Call (_mockAclFinder.Find (_transaction, _context)).Return (CreateAcl (_transaction, _ace));
      Expect.Call (_mockTokenBuilder.CreateToken (_transaction, _principal, _context)).Throw (expectedException);
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_transaction, _context, _principal);

      _mocks.VerifyAll ();
      Assert.AreEqual (0, accessTypes.Length);
      log4net.Core.LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreSame (expectedException, events[0].ExceptionObject);
      Assert.AreEqual (log4net.Core.Level.Error, events[0].Level);
    }

    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      Assert.AreEqual (0, _service.GetRevision ());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (((ISecurityProvider) _service).IsNull);
    }
    
    private AccessControlList CreateAcl (ClientTransaction transaction, AccessControlEntry ace)
    {
      AccessControlList acl = new AccessControlList (transaction);
      acl.AccessControlEntries.Add (ace);

      return acl;
    }

    private AccessControlEntry CreateAce (ClientTransaction transaction)
    {
      AccessControlEntry ace = new AccessControlEntry (transaction);

      AbstractRoleDefinition abstractRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "QualityManager", 0);
      ace.SpecificAbstractRole = abstractRole;

      AccessTypeDefinition readAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Read|MyTypeName", 0);
      AccessTypeDefinition writeAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Write|MyTypeName", 1);
      AccessTypeDefinition deleteAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Delete|MyTypeName", 2);

      ace.AttachAccessType (readAccessType);
      ace.AttachAccessType (writeAccessType);
      ace.AttachAccessType (deleteAccessType);

      ace.AllowAccess (readAccessType);

      return ace;
    }

    private IPrincipal CreateUser ()
    {
      return new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
    }
  }
}
