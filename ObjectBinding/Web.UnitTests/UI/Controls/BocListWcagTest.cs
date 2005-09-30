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
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
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
  [ExpectedException (typeof (WcagException), "The value of property 'ShowOptionsMenu' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasOptionsMenu);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasOptionsMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'ShowListMenu' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowListMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowListMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

  [Test]
  public void IsListMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasListMenu);
  }

  [Test]
  public void IsListMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasListMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'ShowAvailableViewsList' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowAvailableViewsListTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

  [Test]
  public void IsAvailableViewsListInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsFalse (_bocList.HasAvailableViewsList);
  }

  [Test]
  public void IsAvailableViewsListVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsTrue (_bocList.HasAvailableViewsList);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'PageSize' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithPageSizeNotNull()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

  [Test]
  public void IsPagingDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    Assert.IsFalse (_bocList.IsPagingEnabled);
  }

  [Test]
  public void IsPagingEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.PageSize = 1;
    Assert.IsTrue (_bocList.IsPagingEnabled);
  }


  [Test]
  public void IsClientSideSortingEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.EnableSorting = true;
    Assert.IsTrue (_bocList.IsClientSideSortingEnabled);
    // Assert.Success()
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'RowMenuDisplay' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithRowMenuDisplayAutomatic()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.RowMenuDisplay = RowMenuDisplay.Automatic;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Columns[0]' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithDropDownMenuColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {dropDownMenuColumn});
    Assert.Fail();
  }

  [Test]
  public void IsDropDownMenuColumnInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    Assert.IsFalse (_bocList.IsColumnVisible (dropDownMenuColumn));
  }

  [Test]
  public void IsDropDownMenuColumnVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    Assert.IsTrue (_bocList.IsColumnVisible (dropDownMenuColumn));
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Columns[0]' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithEditDetailsColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocEditDetailsColumnDefinition editDetailsColumn = new BocEditDetailsColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {editDetailsColumn});
    Assert.Fail();
  }

  [Test]
  public void IsEditDetailsColumnInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocEditDetailsColumnDefinition editDetailsColumn = new BocEditDetailsColumnDefinition();
    Assert.IsFalse (_bocList.IsColumnVisible (editDetailsColumn));
  }

  [Test]
  public void IsEditDetailsColumnVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocEditDetailsColumnDefinition editDetailsColumn = new BocEditDetailsColumnDefinition();
    Assert.IsTrue (_bocList.IsColumnVisible (editDetailsColumn));
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Columns[0]' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToEvent()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    Assert.Fail();
  }

  [Test]
  public void IsCommandColumnSetToEventInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    Assert.IsFalse (_bocList.IsColumnVisible (commandColumn));
  }

  [Test]
  public void IsCommandColumnSetToEventWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    Assert.IsTrue (_bocList.IsColumnVisible (commandColumn));
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Columns[0]' for BocListMock 'BocList' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToWxeFunction()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    Assert.Fail();
  }

  [Test]
  public void IsCommandColumnSetToWxeFunctionInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    Assert.IsFalse (_bocList.IsColumnVisible (commandColumn));
  }

  [Test]
  public void IsCommandColumnSetToWxeFunctionVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    Assert.IsTrue (_bocList.IsColumnVisible (commandColumn));
  }

	
  [Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToHref()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Href;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command = null;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    // Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Selection' for BocListMock 'BocList' does not comply with a priority 2 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionDisabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.Disabled;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionAndIndexEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.InitialOrder;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    // Assert.Succeed();
  }

}

}
