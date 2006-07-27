using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityProviderTestWithPermissionsFromStaticMethod
  {
    // types

    // static members

    // member fields

    private IWxeSecurityProvider _securityProvider;
    private MockRepository _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _mockSecurityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    // construction and disposing

    public WxeSecurityProviderTestWithPermissionsFromStaticMethod ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WxeSecurityProvider ();

      _mocks = new MockRepository ();

      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _userProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_userProvider.GetUser ()).Return (_user);

      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = null;
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = new FunctionalSecurityStrategy ();
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessType.Search, true);
      _mocks.ReplayAll ();

      _securityProvider.CheckAccess (new TestFunctionWithPermissionsFromStaticMethod ());
      
      _mocks.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessType.Search, false);
      _mocks.ReplayAll ();

      _securityProvider.CheckAccess (new TestFunctionWithPermissionsFromStaticMethod ());
    }

    [Test]
    public void CheckAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      _mocks.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityProvider.CheckAccess (new TestFunctionWithPermissionsFromStaticMethod ());
      }

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessType.Search, true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithPermissionsFromStaticMethod ());

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityProvider.HasAccess (new TestFunctionWithPermissionsFromStaticMethod ());
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessType.Search, false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithPermissionsFromStaticMethod ());

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessType.Search, true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromStaticMethod));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromStaticMethod));
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessType.Search, false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromStaticMethod));

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    private void ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (typeof (SecurableObject), _mockSecurityService, _user, AccessType.Get (accessTypeEnum)))
          .Return (returnValue);
    }
  }
}