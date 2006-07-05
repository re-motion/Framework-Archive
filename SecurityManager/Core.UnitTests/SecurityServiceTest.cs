using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;
using Rubicon.Security;

using Rubicon.SecurityManager.UnitTests.TestDomain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using System.Security.Principal;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests
{
  [TestFixture]
  public class SecurityServiceTest : DomainTest
  {
    [Test]
    public void GetAccess_WithoutAccess ()
    {
      Mockery mocks = new Mockery ();
      IAccessControlListFinder aclFinder = mocks.NewMock<IAccessControlListFinder> ();
      ISecurityTokenBuilder tokenBuilder = mocks.NewMock<ISecurityTokenBuilder> ();

      SecurityService service = new SecurityService (aclFinder, tokenBuilder);
      SecurityContext context = new SecurityContext (typeof (Order), "Owner", "OwnerGroup", "OwnerClient", null, null);
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = CreateAce (transaction);
      IPrincipal principal = CreateUser ();
      SecurityToken token = new SecurityToken (null, new List<Group> (), new List<AbstractRoleDefinition> ());

      Expect.Once.On (tokenBuilder).Method ("CreateToken").With (transaction, principal, context).Will (Return.Value (token));
      Expect.Once.On (aclFinder).Method ("Find").With (transaction, context).Will (Return.Value (CreateAcl (transaction, ace)));

      AccessType[] accessTypes = service.GetAccess (transaction, context, principal);

      mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAccess_WithReadAccess ()
    {
      Mockery mocks = new Mockery ();
      IAccessControlListFinder aclFinder = mocks.NewMock<IAccessControlListFinder> ();
      ISecurityTokenBuilder tokenBuilder = mocks.NewMock<ISecurityTokenBuilder> ();

      SecurityService service = new SecurityService (aclFinder, tokenBuilder);
      SecurityContext context = new SecurityContext (typeof (Order), "Owner", "OwnerGroup", "OwnerClient", null, null);
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = CreateAce (transaction);
      IPrincipal principal = CreateUser ();

      List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition> ();
      roles.Add (ace.SpecificAbstractRole);
      SecurityToken token = new SecurityToken (null, new List<Group> (), roles);

      Expect.Once.On (tokenBuilder).Method ("CreateToken").With (transaction, principal, context).Will (Return.Value (token));
      Expect.Once.On (aclFinder).Method ("Find").With (transaction, context).Will (Return.Value (CreateAcl (transaction, ace)));

      AccessType[] accessTypes = service.GetAccess (transaction, context, principal);

      mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (EnumWrapper.Parse ("Read|MyTypeName")), accessTypes);
    }

    [Test]
    public void GetAccess_WithReadAccessFromInterface ()
    {
      Mockery mocks = new Mockery ();
      IAccessControlListFinder aclFinder = mocks.NewMock<IAccessControlListFinder> ();
      ISecurityTokenBuilder tokenBuilder = mocks.NewMock<ISecurityTokenBuilder> ();

      ISecurityService service = new SecurityService (aclFinder, tokenBuilder);
      SecurityContext context = new SecurityContext (typeof (Order), "Owner", "OwnerGroup", "OwnerClient", null, null);
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = CreateAce (transaction);
      IPrincipal principal = CreateUser ();

      List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition> ();
      roles.Add (ace.SpecificAbstractRole);
      SecurityToken token = new SecurityToken (null, new List<Group> (), roles);

      Expect.Once.On (tokenBuilder).Method ("CreateToken").With (Is.NotNull, Is.Same (principal), Is.Same (context)).Will (Return.Value (token));
      Expect.Once.On (aclFinder).Method ("Find").With (Is.NotNull, Is.Same (context)).Will (Return.Value (CreateAcl (transaction, ace)));

      AccessType[] accessTypes = service.GetAccess (context, principal);

      mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (EnumWrapper.Parse ("Read|MyTypeName")), accessTypes);
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
