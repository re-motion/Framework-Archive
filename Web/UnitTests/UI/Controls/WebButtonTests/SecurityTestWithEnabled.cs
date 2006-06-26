using System;
using System.Collections.Generic;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : WebButtonTest
  {
    private MockRepository _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private ISecurableObject _mockSecurableObject;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityProvider = _mocks.CreateMock<IWebSecurityProvider> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();

      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_mockWebSecurityProvider);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      Expect.Call (_mockWebSecurityProvider.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (true);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Call (_mockWebSecurityProvider.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (false);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject;

      return button;
    }
  }
}
