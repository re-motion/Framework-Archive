using System;
using System.Collections.Generic;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls.WebMenuItemTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private ISecurableObject _mockSecurableObject;
    private Command _mockCommand;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityProvider = _mocks.CreateMock<IWebSecurityProvider> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();
      _mockCommand = _mocks.CreateMock<Command> ();

      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_mockWebSecurityProvider);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsDisabled = false;

      bool isEnabled = menuItem.EvaluateEnabled ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsDisabled = true;

      bool isEnabled = menuItem.EvaluateEnabled ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (true);
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (false);
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = true;
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }

    private WebMenuItem CreateWebMenuItem ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.Command = _mockCommand;
      
      return menuItem;
    }

    private WebMenuItem CreateWebMenuItemWithoutCommand ()
    {
      WebMenuItem menuItem = new WebMenuItem ();
      menuItem.Command.Type = CommandType.WxeFunction;
      menuItem.Command = null;
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.SecurableObject = _mockSecurableObject;

      return menuItem;
    }
  }
}
