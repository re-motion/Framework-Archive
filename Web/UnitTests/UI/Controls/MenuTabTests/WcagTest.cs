using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class WcagTest : BaseTest
  {
    [Test]
    public void IsMainMenuTabSetToEventInvisibleWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA ();
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.Event;
      Assert.IsFalse (mainMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsMainMenuTabSetToEventVisibleWithoutWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined ();
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.Event;
      Assert.IsTrue (mainMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsSubMenuTabSetToEventInvisibleWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA ();
      SubMenuTab subMenuTab = new SubMenuTab ();
      subMenuTab.Command.Type = CommandType.Event;
      Assert.IsFalse (subMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsSubMenuTabSetToEventVisibleWithoutWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined ();
      SubMenuTab subMenuTab = new SubMenuTab ();
      subMenuTab.Command.Type = CommandType.Event;
      Assert.IsTrue (subMenuTab.EvaluateVisible ());
    }
  }
}
