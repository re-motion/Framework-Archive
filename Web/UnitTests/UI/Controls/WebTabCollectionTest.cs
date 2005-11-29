using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.UnitTests.UI;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls
{

[TestFixture]
public class WebTabCollectionTest: WebControlTest
{
  private WebTabStrip _tabStrip;
  private WebTab _tab0;
  private WebTab _tab1;
  private WebTab _tab2;  
  private WebTab _tab3;
  private WebTab _tabNew;

  public WebTabCollectionTest()
  {
  }
  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _tabStrip = new WebTabStrip();
    _tab0 = new WebTab ("Tab0", "Tab 0");
    _tab1 = new WebTab ("Tab1", "Tab 1");
    _tab2 = new WebTab ("Tab2", "Tab 2");
    _tab3 = new WebTab ("Tab3", "Tab 3");
    _tabNew = new WebTab ("Tab5", "Tab 5");
  }

  [Test]
  public void AddTabs()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tab1.IsSelected = true;
    _tabStrip.Tabs.Add (_tab3);
 
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.IsNotNull (_tabStrip.SelectedTab);
    Assert.AreSame (_tab1, _tabStrip.SelectedTab);
  }

  [Test]
  public void InsertFirstTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert (0, _tabNew);
    
    Assert.AreEqual (5, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void InsertMiddleTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert (2, _tabNew);
    
    Assert.AreEqual (5, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[2], "Wrong tab at position 2.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void InsertLastTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert (4, _tabNew);
    
    Assert.AreEqual (5, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[4], "Wrong tab at position 4.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceFirstTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceMiddleTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs[1] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[1], "Wrong tab at position 1.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab3, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceLastTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[3] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[3], "Wrong tab at position 3.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab0, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceFirstTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab1, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceMiddleTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab1.IsSelected = true;

    _tabStrip.Tabs[1] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[1], "Wrong tab at position 1.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceLastTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs[3] = _tabNew;
    
    Assert.AreEqual (4, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[3], "Wrong tab at position 3.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void ReplaceOnlyTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;

    Assert.AreEqual (1, _tabStrip.Tabs.Count);
    Assert.AreSame (_tabNew, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    Assert.IsNull (_tabStrip.SelectedTab, "Tab selected.");
  }

  [Test]
  public void RemoveFirstTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (0);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    Assert.AreSame (_tab1, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveMiddleTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (1);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    Assert.AreSame (_tab2, _tabStrip.Tabs[1], "Wrong tab at position 1.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab3, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveLastTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (3);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab0, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveFirstTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (0);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    Assert.AreSame (_tab1, _tabStrip.Tabs[0], "Wrong tab at position 0.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab1, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveMiddleTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab1.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (1);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    Assert.AreSame (_tab2, _tabStrip.Tabs[1], "Wrong tab at position 1.");
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveLastTabWithTabBeingSelected()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tabStrip.Tabs.Add (_tab1);
    _tabStrip.Tabs.Add (_tab2);
    _tabStrip.Tabs.Add (_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (3);
    
    Assert.AreEqual (3, _tabStrip.Tabs.Count);
    
    Assert.IsNotNull (_tabStrip.SelectedTab, "No tab selected.");
    Assert.AreSame (_tab2, _tabStrip.SelectedTab, "Wrong tab selected.");
  }

  [Test]
  public void RemoveOnlyTab()
  {
    _tabStrip.Tabs.Add (_tab0);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt (0);

    Assert.AreEqual (0, _tabStrip.Tabs.Count);
    Assert.IsNull (_tabStrip.SelectedTab, "Tab selected.");
  }

}
}
