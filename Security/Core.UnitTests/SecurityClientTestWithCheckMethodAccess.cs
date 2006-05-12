using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTestWithCheckMethodAccess
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private IPrincipal _user;
    private SecurityContext _context;
    private IPermissionReflector _permissionReflectorMock;
    private ISecurityContextFactory _contextFactoryMock;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionReflector> ();
      _contextFactoryMock = _mocks.NewMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableClass), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Stub.On (_contextFactoryMock)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));
    }

    [Test]
    public void CheckSuccessfulAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableClass), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableClass (_contextFactoryMock), "Record", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (AccessViolationException))]
    public void CheckDeniedAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableClass), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableClass (_contextFactoryMock), "Record", _user);
    }

    [Test]
    public void CheckAccessForOverloadedMethod ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableClass), "Load", new Type[] { typeof (string) })
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableClass (_contextFactoryMock), "Load", new Type[] { typeof (string) }, _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
