using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Web.UI;
using Rubicon.Security.Web.UnitTests.ExecutionEngine;
using Rubicon.Security.Configuration;
using System.Security.Principal;
using Rubicon.Web.UI;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Security.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.UI.WebSecurityProviderTests
{
  [TestFixture]
  public class PermissionFromWxeFunctionTest
  {
    private IWebSecurityProvider _securityProvider;
    private Mockery _mocks;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WebSecurityProvider ();

      _mocks = new Mockery ();

      _mockWxeSecurityProvider = _mocks.NewMock<IWxeSecurityProvider> ();
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_mockWxeSecurityProvider);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);
    }

    [Test]
    public void HasAccessGranted ()
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (typeof (TestFunctionWithThisObject))
         .Will (Return.Value (true));

      WebSecurityProvider securityProvider = new WebSecurityProvider ();

      bool hasAccess = securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (typeof (TestFunctionWithThisObject))
         .Will (Return.Value (false));

      WebSecurityProvider securityProvider = new WebSecurityProvider ();

      bool hasAccess = securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetWxeFunctionPermission (typeof (TestFunctionWithThisObject))]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }

  }
}