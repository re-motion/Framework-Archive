using System;
using System.Security.Principal;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Security
{

[TestFixture]
public class WxeFunctionSecurityTest: WxeTest
{
  private Mockery _mocks;
  private ISecurityService _securityService;
  private IUserProvider _userProvider;
  private SecurityContext _securityContext;
  private ISecurityContextFactory _securityContextFactory;

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp ();

    _mocks = new Mockery ();
    _securityService = _mocks.NewMock<ISecurityService> ();
    _userProvider = _mocks.NewMock<IUserProvider> ();
    SecurityConfiguration.Current.SecurityService = _securityService;
    SecurityConfiguration.Current.UserProvider = _userProvider;
    Stub.On (_userProvider)
        .Method ("GetUser")
        .Will (Return.Value (new GenericPrincipal (new GenericIdentity ("owner"), new string[0])));

    _securityContext = new SecurityContext (typeof (SecurableClass), "owner", "group", "client", null, null);

    _securityContextFactory = _mocks.NewMock<ISecurityContextFactory> ();
    Stub.On (_securityContextFactory)
        .Method ("GetSecurityContext")
        .Will (Return.Value (_securityContext));
  }

  [Test]
  public void ExecuteFunctionWithoutSecurity ()
  {
    Expect.Never.On (_securityService)
        .Method ("GetAccess");

    TestFunction function = new TestFunction ();
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }

  [Test]
  public void ExecuteFunctionWithPermissionsFromInstanceMethod ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read) }));

    TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (new SecurableClass (_securityContextFactory));
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }

  [Test]
  [ExpectedException (typeof (PermissionDeniedException))]
  public void ExecuteFunctionWithInsufficientPermissionsFromInstanceMethod ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[0]));

    TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (new SecurableClass (_securityContextFactory));
    function.Execute ();
  }

  [Test]
  public void ExecuteFunctionWithPermissionsFromStaticMethod ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

    TestFunctionWithPermissionsFromStaticMethod function = new TestFunctionWithPermissionsFromStaticMethod ();
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }

  [Test]
  [ExpectedException (typeof (PermissionDeniedException))]
  public void ExecuteFunctionWithInsufficientPermissionsFromStaticMethod ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[0]));

    TestFunctionWithPermissionsFromConstructor function = new TestFunctionWithPermissionsFromConstructor ();
    function.Execute ();
  }

  [Test]
  public void ExecuteFunctionWithPermissionsFromConstructor ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

    TestFunctionWithPermissionsFromConstructor function = new TestFunctionWithPermissionsFromConstructor ();
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }

  [Test]
  [ExpectedException (typeof (PermissionDeniedException))]
  public void ExecuteFunctionWithInsufficientPermissionsFromConstructor ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[0]));

    TestFunctionWithPermissionsFromConstructor function = new TestFunctionWithPermissionsFromConstructor ();
    function.Execute ();
  }
}

}
