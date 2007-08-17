using System;
using NUnit.Framework;
using Rubicon.Security.Configuration;
using Rubicon.Security.Web.UI;
using Rubicon.Security.UnitTests.Web.Configuration;
using Rubicon.Security.UnitTests.Web.Domain;
using Rubicon.Web.UI;

namespace Rubicon.Security.UnitTests.Web.UI.WebSecurityAdapterTests
{
  [TestFixture]
  public class PermissionFromSecurableObjectTest
  {
    private IWebSecurityAdapter _securityAdapter;
    private WebPermissionProviderTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WebSecurityAdapter ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      _testHelper = new WebPermissionProviderTestHelper ();
      SecurityConfiguration.Current.SecurityProvider = _testHelper.SecurityProvider;
      SecurityConfiguration.Current.UserProvider = _testHelper.UserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _testHelper.FunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void HasAccessGranted_WithoutHandler ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), null);

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
        hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessGranted_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetMethodPermission (SecurableObject.Method.Show)]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}