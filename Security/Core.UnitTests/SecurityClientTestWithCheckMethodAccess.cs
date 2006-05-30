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
    private IPermissionProvider _permissionReflectorMock;
    private ISecurityContextFactory _contextFactoryMock;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionProvider> ();
      _contextFactoryMock = _mocks.NewMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Stub.On (_contextFactoryMock)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));

      _securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
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

      _securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
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

      _securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), "The method 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForMethodWithoutRequiredPermissions ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Save")
          .Will (Return.Value (new Enum[0]));

      _securityClient.CheckMethodAccess (new SecurableObject (_contextFactoryMock), "Save", _user);
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

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

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

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    [Test, ExpectedException (typeof (ArgumentException),
        "The method 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForStaticMethodWithoutRequiredPermissions ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[0]));

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
