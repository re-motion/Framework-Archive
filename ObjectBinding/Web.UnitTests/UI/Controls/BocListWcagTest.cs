using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocListWcagTest
{
  private BocListMock _bocList;

  [SetUp]
  public virtual void SetUp()
  {
    _bocList = new BocListMock();
    _bocList.ID = "BocList";
    _bocList.ShowOptionsMenu = false;
    _bocList.ShowListMenu = false;
    _bocList.ShowAvailableViewsList = false;
    _bocList.PageSize = NaInt32.Null;
    _bocList.EnableSorting = false;
    _bocList.RowMenuDisplay = RowMenuDisplay.Disabled;
    _bocList.Selection = RowSelection.Disabled;
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelUndefined();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property ShowOptionsMenu of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAShowOptionsMenuFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowOptionsMenu = false;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

  [Test]
  public void HasOptionsMenuWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasOptionsMenu);
  }

  [Test]
  public void HasOptionsMenuWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasOptionsMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property ShowListMenu of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowListMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowListMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAShowListMenuFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowListMenu = false;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

  [Test]
  public void HasListMenuWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasListMenu);
  }

  [Test]
  public void HasListMenuWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasListMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property ShowAvailableViewsList of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowAvailableViewsListTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAShowAvailableViewsListFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.ShowAvailableViewsList = false;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

  [Test]
  public void HasAvailableViewsListWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsFalse (_bocList.HasAvailableViewsList);
  }

  [Test]
  public void HasAvailableViewsListWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsTrue (_bocList.HasAvailableViewsList);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property PageSize of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithPageSizeNotNull()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithPageSizeNull()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.PageSize = NaInt32.Null;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property EnableSorting of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithEnableSortingTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.EnableSorting = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithEnableSortingFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.EnableSorting = false;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property RowMenuDisplay of Control BocList does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithRowMenuDisplayAutomatic()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.RowMenuDisplay = RowMenuDisplay.Automatic;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithRowMenuDisplayDisabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocList.RowMenuDisplay = RowMenuDisplay.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "Property Selection of Control BocList does comply with a priority 2 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionDisabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelDoubleA();
    _bocList.Selection = RowSelection.Disabled;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionAndIndexEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.InitialOrder;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }
}

}
