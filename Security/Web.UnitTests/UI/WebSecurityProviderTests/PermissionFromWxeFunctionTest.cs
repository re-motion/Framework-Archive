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
    private WebPermissionProviderTestHelper _testHelper;
  
    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WebSecurityProvider ();

      _testHelper = new WebPermissionProviderTestHelper ();
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasStatelessAccess (typeof (TestFunctionWithThisObject), true);

      bool hasAccess = _securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasStatelessAccess (typeof (TestFunctionWithThisObject), false);

      bool hasAccess = _securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetWxeFunctionPermission (typeof (TestFunctionWithThisObject))]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }

  }
}