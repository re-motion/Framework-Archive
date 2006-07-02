using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasMethodAccessTest
  {
    private MockRepository _mocks;
    private ISecurityService _securityServiceMock;
    private IPrincipal _user;
    private SecurityContext _context;
    private IPermissionProvider _permissionReflectorMock;
    private ISecurityContextFactory _contextFactoryMock;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _securityServiceMock = _mocks.CreateMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.CreateMock<IPermissionProvider> ();
      _contextFactoryMock = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      SetupResult.For (_contextFactoryMock.GetSecurityContext ()).Return (_context);

      _securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    [Test]
    public void HasSuccessfulAccess ()
    {
      SetupRecordMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Edit);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);
      
      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedAccess ()
    {
      SetupRecordMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Read);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        "The member 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void HasAccessForMethodWithoutRequiredPermissions ()
    {
      SetupSaveMethodAccessTypes ();
      _mocks.ReplayAll ();

      _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Save", _user);
    }

    [Test]
    public void HasSuccessfulStaticMethodAccess ()
    {
      SetupCreateForSpecialCaseMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Edit);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedAccessForStaticMethod ()
    {
      SetupCreateForSpecialCaseMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Read);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The member 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void HasAccessForStaticMethodWithoutRequiredPermissions ()
    {
      SetupCreateForSpecialCaseMethodAccessTypes ();
      _mocks.ReplayAll ();

      _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    [Test]
    public void HasSuccessfulStatelessAccess ()
    {
      SetupRecordMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Edit);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "Record", _user);

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedStatelessAccess ()
    {
      SetupRecordMethodAccessTypes (GeneralAccessType.Edit);
      SetupSecurityServiceResult (GeneralAccessType.Read);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "Record", _user);

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The member 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void HasStatelessAccessForMethodWithoutRequiredPermissions ()
    {
      SetupSaveMethodAccessTypes ();
      _mocks.ReplayAll ();

      _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "Save", _user);
    }

    private void SetupRecordMethodAccessTypes (params Enum[] accessTypes)
    {
      Expect.Call (_permissionReflectorMock.GetRequiredMethodPermissions (typeof (SecurableObject), "Record")).Return (accessTypes);
    }

    private void SetupSaveMethodAccessTypes (params Enum[] accessTypes)
    {
      Expect.Call (_permissionReflectorMock.GetRequiredMethodPermissions (typeof (SecurableObject), "Save")).Return (accessTypes);
    }

    private void SetupCreateForSpecialCaseMethodAccessTypes (params Enum[] accessTypes)
    {
      Expect.Call (_permissionReflectorMock.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase")).Return (accessTypes);
    }

    private void SetupSecurityServiceResult (params Enum[] accessTypeEnums)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Call (_securityServiceMock.GetAccess (_context, _user)).IgnoreArguments().Return (accessTypes);
    }
  }
}
