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
  public class SecurityTestWithVisible
  {
    private WebButton _button;
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
      _button = new WebButton ();
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.SecurableObject = _mockSecurableObject;
    }

    [Test]
    public void SecurityDepenedentPropertySetToVisible ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
       Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .With (_mockSecurableObject, new EventHandler (TestHandler))
          .Will (Return.Value (true));

      _button.Click += TestHandler;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .With (_mockSecurableObject, new EventHandler (TestHandler))
          .Will (Return.Value (false));

      _button.Click += TestHandler;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }
  }
}
