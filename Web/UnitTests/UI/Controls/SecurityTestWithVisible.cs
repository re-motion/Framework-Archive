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
    private Mockery _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private WebButton _button;

    [SetUp]
    public void Setup ()
    {
      _mocks = new Mockery ();
      _mockWebSecurityProvider = _mocks.NewMock<IWebSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_mockWebSecurityProvider);
      _button = new WebButton ();
    }

    [Test]
    public void SecurityDepenedentPropertySetToEnabled ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Click += TestHandler;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void SecurityDepenedentPropertySetToVisible ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Click += TestHandler;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void VisibleWithoutWebSecurityProviderAndVisibleTrue ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void VisibleWithoutWebSecurityProviderAndVisibleFalse ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void VisibleWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void VisibleWithoutClickEventHandlerAndVisibleFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void VisibleWithAccessGrantedAndVisibleTrue ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .WithAnyArguments ()
          .Will (Return.Value (true));

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void VisibleWithVisibleFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = false;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void VisibleWithAccessDeniedAndVisibleTrue ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .WithAnyArguments ()
          .Will (Return.Value (false));

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Visible;
      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EnabledWithoutWebSecurityProviderAndVisibleTrue ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EnabledWithoutWebSecurityProviderAndVisibleFalse ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = false;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EnabledWithoutClickEventHandler ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EnabledWithoutClickEventHandlerAndVisibleFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = false;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EnabledWithAccessGrantedAndVisibleTrue ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .WithAnyArguments ()
          .Will (Return.Value (true));

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EnabledWithVisibleFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = false;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EnabledWithAccessDeniedAndVisibleTrue ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
          .Method ("HasAccess")
          .WithAnyArguments ()
          .Will (Return.Value (false));

      _button.Click += TestHandler;
      _button.SecurityDependentProperty = SecurityDependentProperty.Enabled;
      _button.Enabled = true;
      bool enabled = _button.Enabled;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (enabled);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }
  }
}
