using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Web.UnitTests.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Security.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityProviderTestWithoutPermissionAttribute
  {
    // types

    // static members

    // member fields

    private IWxeSecurityProvider _securityProvider;
    private MockRepository _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _mockSecurityService;
    private IUserProvider _mockUserProvider;

    // construction and disposing

    public WxeSecurityProviderTestWithoutPermissionAttribute ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WxeSecurityProvider ();

      _mocks = new MockRepository ();

      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      SetupResult.For (_mockSecurityService.IsNull).Return (false);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = new NullSecurityService ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = new FunctionalSecurityStrategy ();
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      _securityProvider.CheckAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();
      
      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithoutPermissions));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}