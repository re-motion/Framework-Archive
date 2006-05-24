using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;

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
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Stub.On (_contextFactoryMock)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));
    }

    [Test]
    public void CheckSuccessfulAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);
    }

    //[Test]
    //public void CheckAccessForOverloadedMethod ()
    //{
    //  Expect.Once.On (_permissionReflectorMock)
    //      .Method ("GetRequiredMethodPermissions")
    //      .With (typeof (SecurableObject), "Load", new Type[] { typeof (string) })
    //      .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
    //  Expect.Once.On (_securityServiceMock)
    //      .Method ("GetAccess")
    //      .With (_context, _user)
    //      .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

    //  SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
    //  securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Load", new Type[] { typeof (string) }, _user);

    //  _mocks.VerifyAllExpectationsHaveBeenMet ();
    //}

    [Test]
    [ExpectedException(typeof(ArgumentException), "The method 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForMethodWithoutRequiredPermissions ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Save")
          .Will (Return.Value (new Enum[0]));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Save", _user);
    }

    [Test]
    public void CheckSuccessfulStaticMethodAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccessForStaticMethod ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    //[Test]
    //public void CheckAccessForOverloadedStaticMethod ()
    //{
    //  Expect.Once.On (_permissionReflectorMock)
    //      .Method ("GetRequiredStaticMethodPermissions")
    //      .With (typeof (SecurableObject), "IsValid", new Type[] { typeof (SecurableObject) })
    //      .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
    //  Expect.Once.On (_securityServiceMock)
    //      .Method ("GetAccess")
    //      .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

    //  SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
    //  securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "IsValid", new Type[] { typeof (SecurableObject) }, _user);

    //  _mocks.VerifyAllExpectationsHaveBeenMet ();
    //}

    [Test, ExpectedException (typeof (ArgumentException),
        "The method 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForStaticMethodWithoutRequiredPermissions ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[0]));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
