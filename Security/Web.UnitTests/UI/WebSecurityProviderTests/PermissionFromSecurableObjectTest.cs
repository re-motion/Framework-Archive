using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web.UI;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Configuration;
using Rubicon.Security.Web.UI;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.UI;

namespace Rubicon.Security.Web.UnitTests.UI.WebSecurityProviderTests
{
  [TestFixture]
  public class PermissionFromSecurableObjectTest
  {
    private IWebSecurityProvider _securityProvider;
    private WebPermissionProviderTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WebSecurityProvider ();

      _testHelper = new WebPermissionProviderTestHelper ();
      SecurityConfiguration.Current.SecurityService = _testHelper.SecurityService;
      SecurityConfiguration.Current.UserProvider = _testHelper.UserProvider;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = null;
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessType.Read }, true);

      bool hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessType.Read }, false);

      bool hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetMethodPermission (SecurableObject.Methods.Show)]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}