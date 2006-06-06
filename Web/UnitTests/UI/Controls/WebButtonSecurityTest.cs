using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class WebButtonSecurityTest
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
    public void VisibleWithoutWebSecurityProviderAndVisibleTrue ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

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

      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void VisibleWithVisibleFalse ()
    {
      Expect.Never.On (_mockWebSecurityProvider);

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

      _button.Visible = true;
      bool isVisible = _button.Visible;

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isVisible);
    }
  }
}
