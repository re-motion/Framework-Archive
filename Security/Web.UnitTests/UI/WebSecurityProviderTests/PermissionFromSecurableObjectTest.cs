using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web.UI;

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
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _testHelper.FunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = new NullSecurityService();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = new FunctionalSecurityStrategy();
    }

    [Test]
    public void HasAccessGranted_WithoutHandler ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), null);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessGranted_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetMethodPermission (SecurableObject.Method.Show)]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}