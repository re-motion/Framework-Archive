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
  public class SecurityTestWithVisible : WebButtonTest
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
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      Expect.Call (_mockWebSecurityProvider.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (true);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Call (_mockWebSecurityProvider.HasAccess(_mockSecurableObject, new EventHandler (TestHandler))).Return (false);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.SecurableObject = _mockSecurableObject;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.SecurableObject = _mockSecurableObject;

      return button;
    }
  }
}
