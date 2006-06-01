using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NMock2;
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
    private Mockery _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;

    // construction and disposing

    public WxeSecurityProviderTestWithoutPermissionAttribute ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WxeSecurityProvider ();

      _mocks = new Mockery ();

      _securityService = _mocks.NewMock<ISecurityService> ();
      _userProvider = _mocks.NewMock<IUserProvider> ();
      _mockFunctionalSecurityStrategy = _mocks.NewMock<IFunctionalSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [Test]
    public void CheckAccess ()
    {
      Expect.Never.On (_securityService);
      Expect.Never.On (_userProvider);
      Expect.Never.On (_mockFunctionalSecurityStrategy);
      
      _securityProvider.CheckAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Never.On (_securityService);
      Expect.Never.On (_userProvider);
      Expect.Never.On (_mockFunctionalSecurityStrategy);

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess ()
    {
      Expect.Never.On (_securityService);
      Expect.Never.On (_userProvider);
      Expect.Never.On (_mockFunctionalSecurityStrategy);
      
      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithoutPermissions));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }
  }
}