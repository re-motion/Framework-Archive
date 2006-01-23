using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls
{

[TestFixture]
public class TabbedMenuTest: WebControlTest
{
  private HttpContext _currentHttpContext;

  private TabbedMenuMock _tabbedMenu;

  private MainMenuTab _mainMenuTab1;
  private MainMenuTab _mainMenuTab2;
  private MainMenuTab _mainMenuTab3;

  private SubMenuTab _subMenuTab11;
  private SubMenuTab _subMenuTab12;
  private SubMenuTab _subMenuTab13;

  private SubMenuTab _subMenuTab21;
  private SubMenuTab _subMenuTab22;
  private SubMenuTab _subMenuTab23;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _tabbedMenu = new TabbedMenuMock();
    _tabbedMenu.ID = "TabbedMenu";

    _mainMenuTab1 = new MainMenuTab("MainMenuTab1", "Main 1");
    _mainMenuTab2 = new MainMenuTab("MainMenuTab2", "Main 2");
    _mainMenuTab3 = new MainMenuTab("MainMenuTab3", "Main 3");

    _subMenuTab11 = new SubMenuTab("SubMenuTab11", "Sub 1.1");
    _subMenuTab12 = new SubMenuTab("SubMenuTab12", "Sub 1.2");
    _subMenuTab13 = new SubMenuTab("SubMenuTab13", "Sub 1.3");

    _subMenuTab21 = new SubMenuTab("SubMenuTab21", "Sub 2.1");
    _subMenuTab22 = new SubMenuTab("SubMenuTab22", "Sub 2.2");
    _subMenuTab23 = new SubMenuTab("SubMenuTab23", "Sub 2.3");

    _mainMenuTab1.SubMenuTabs.Add (_subMenuTab11);
    _mainMenuTab1.SubMenuTabs.Add (_subMenuTab12);
    _mainMenuTab1.SubMenuTabs.Add (_subMenuTab13);

    _mainMenuTab2.SubMenuTabs.Add (_subMenuTab21);
    _mainMenuTab2.SubMenuTabs.Add (_subMenuTab22);
    _mainMenuTab2.SubMenuTabs.Add (_subMenuTab23);

    _tabbedMenu.Tabs.Add (_mainMenuTab1);
    _tabbedMenu.Tabs.Add (_mainMenuTab2);
    _tabbedMenu.Tabs.Add (_mainMenuTab3);
  }

  [Test]
  public void GetUrlParametersForMainMenuTab()
  {
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_mainMenuTab2.ItemID});
    
    NameValueCollection parameters = _tabbedMenu.GetUrlParameters (_mainMenuTab2);

    Assert.IsNotNull (parameters);
    Assert.AreEqual (1, parameters.Count);
    Assert.IsNotNull (parameters.GetKey (0));
    Assert.AreEqual (_tabbedMenu.SelectionID, parameters.GetKey (0));
    Assert.IsNotNull (parameters.Get (0));
    Assert.AreEqual (expectedParameterValue, parameters.Get (0));
  }

  [Test]
  public void GetUrlParametersForSubMenuTab()
  {
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_subMenuTab22.Parent.ItemID, _subMenuTab22.ItemID});
    
    NameValueCollection parameters = _tabbedMenu.GetUrlParameters (_subMenuTab22);

    Assert.IsNotNull (parameters);
    Assert.AreEqual (1, parameters.Count);
    Assert.IsNotNull (parameters.GetKey (0));
    Assert.AreEqual (_tabbedMenu.SelectionID, parameters.GetKey (0));
    Assert.IsNotNull (parameters.Get (0));
    Assert.AreEqual (expectedParameterValue, parameters.Get (0));
  }

  [Test]
  public void FormatUrlForMainMenuTab()
  {
    string url = "/AppDir/page.aspx";
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_mainMenuTab2.ItemID});
    string expectedUrl = UrlUtility.AddParameter (url, _tabbedMenu.SelectionID, expectedParameterValue);
    
    string value = _tabbedMenu.FormatUrl (url, _mainMenuTab2);

    Assert.IsNotNull (value);
    Assert.AreEqual (expectedUrl, value);
  }

  [Test]
  public void FormatUrlForSubMenuTab()
  {
    string url = "/AppDir/page.aspx";
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_subMenuTab22.Parent.ItemID, _subMenuTab22.ItemID});
    string expectedUrl = UrlUtility.AddParameter (url, _tabbedMenu.SelectionID, expectedParameterValue);
    
    string value = _tabbedMenu.FormatUrl (url, _subMenuTab22);

    Assert.IsNotNull (value);
    Assert.AreEqual (expectedUrl, value);
  }

  [Test]
  public void FormatUrlForSelectedMainMenuTab()
  {
    string url = "/AppDir/page.aspx";
    _mainMenuTab3.IsSelected = true;
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_mainMenuTab3.ItemID});
    string expectedUrl = UrlUtility.AddParameter (url, _tabbedMenu.SelectionID, expectedParameterValue);
    
    string value = _tabbedMenu.FormatUrl (url);

    Assert.IsNotNull (value);
    Assert.AreEqual (expectedUrl, value);
  }

  [Test]
  public void FormatUrlForSelectedSubMenuTab()
  {
    string url = "/AppDir/page.aspx";
    _subMenuTab12.IsSelected = true;
    string expectedParameterValue = (string) TypeConversionServices.Current.Convert (
        typeof (string[]), typeof (string), new string[] {_subMenuTab12.Parent.ItemID, _subMenuTab12.ItemID});
    string expectedUrl = UrlUtility.AddParameter (url, _tabbedMenu.SelectionID, expectedParameterValue);
    
    string value = _tabbedMenu.FormatUrl (url);

    Assert.IsNotNull (value);
    Assert.AreEqual (expectedUrl, value);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _mainMenuTab1.Command.Type = CommandType.Event;
    _subMenuTab11.Command.Type = CommandType.Event;
    
    _tabbedMenu.EvaluateWaiConformity();

    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithMainMenuTabSetToEvent()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _tabbedMenu.Tabs.Clear();
    _tabbedMenu.Tabs.Add (_mainMenuTab1);
    _mainMenuTab1.Command.Type = CommandType.Event;

    _tabbedMenu.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsTrue (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithSubMenuTabSetToEvent()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _tabbedMenu.Tabs.Clear();
    _tabbedMenu.Tabs.Add (_mainMenuTab1);
    _mainMenuTab1.Command.Type = CommandType.Event;

    _tabbedMenu.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsTrue (WcagHelperMock.HasError);
  }
}

}
