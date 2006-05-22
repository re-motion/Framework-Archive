using System;
using System.Security.Principal;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
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
  public void ExecuteSecuredFunction ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read) }));

    TestFunctionWithSecurity function = new TestFunctionWithSecurity (new SecurableClass (_securityContextFactory));
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }

  [Test]
  [ExpectedException (typeof (PermissionDeniedException))]
  public void ExecuteSecuredFunctionWithInsufficientPermissions ()
  {
    Expect.Once.On (_securityService)
        .Method ("GetAccess")
        .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

    TestFunctionWithSecurity function = new TestFunctionWithSecurity (new SecurableClass (_securityContextFactory));
    function.Execute ();

    _mocks.VerifyAllExpectationsHaveBeenMet ();
  }
}

}
