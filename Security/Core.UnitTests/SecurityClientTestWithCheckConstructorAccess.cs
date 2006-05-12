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
  public class SecurityClientTestWithCheckConstructorAccess
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private IPermissionReflector _permissionReflectorMock;
    private IPrincipal _user;
    private SecurityContext _statelessContext;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionReflector> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _statelessContext = new SecurityContext (typeof (SecurableClass));
    }

    [Test]
    public void CheckSuccessfulAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredConstructorPermissions")
          .With (typeof (SecurableClass))
          .Will (Return.Value (new Enum[0]));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckConstructorAccess (typeof (SecurableClass), _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (AccessViolationException))]
    public void CheckDeniedAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredConstructorPermissions")
          .With (typeof (SecurableClass))
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckConstructorAccess (typeof (SecurableClass), _user);
    }

    [Test]
    public void CheckAccessForOverloadedConstructor ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredConstructorPermissions")
          .With (typeof (SecurableClass), new Type[] { typeof (string) })
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Create) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock);
      securityClient.CheckConstructorAccess (typeof (SecurableClass), new Type[] { typeof (string) }, _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
