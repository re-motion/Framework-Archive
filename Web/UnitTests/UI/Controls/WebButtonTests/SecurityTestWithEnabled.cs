using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class SecurityTestWithEnabled
  {
    private Mockery _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private ISecurableObject _mockSecurableObject;

    [SetUp]
    public void Setup ()
    {
      _mocks = new Mockery ();
      _mockWebSecurityProvider = _mocks.NewMock<IWebSecurityProvider> ();
      _mockSecurableObject = _mocks.NewMock<ISecurableObject> ();

      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_mockWebSecurityProvider);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithSecurityDepenedentPropertySetToVisible ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithClickEventHandler ();
      button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      button.Enabled = true;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithSecurityDepenedentPropertySetToVisible ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithClickEventHandler ();
      button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      button.Enabled = false;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = true;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = false;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .With (_mockSecurableObject, new EventHandler (TestHandler))
          .Will (Return.Value (true));

      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .With (_mockSecurableObject, new EventHandler (TestHandler))
          .Will (Return.Value (false));

      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      bool enabled = button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      button.SecurableObject = _mockSecurableObject;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      button.SecurableObject = _mockSecurableObject;

      return button;
    }
  }
}
