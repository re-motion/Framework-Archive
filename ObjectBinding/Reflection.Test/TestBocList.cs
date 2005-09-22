using System;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{

public class TestBocList: BocList
{

  protected override WebMenuItem[] InitializeRowMenusItems(IBusinessObject businessObject, int originalRowIndex)
  {
    WebMenuItem[] baseMenuItems = base.InitializeRowMenusItems (businessObject, originalRowIndex);

    WebMenuItem[] menuItems = new WebMenuItem[2];
    WebMenuItem menuItem = new WebMenuItem();
    menuItem.ItemID = originalRowIndex.ToString() + "_0";
    menuItem.Text = menuItem.ItemID;
    menuItems[0] = menuItem;

    menuItem = new TestBocMenuItem (businessObject);
    menuItem.ItemID = originalRowIndex.ToString() + "_1";
    menuItem.Text = menuItem.ItemID;
    menuItems[1] = menuItem;

    return (WebMenuItem[]) ArrayUtility.Combine (baseMenuItems, menuItems);
  }

}

public class TestBocMenuItem: BocMenuItem
{
  private IBusinessObject _businessObject;

  public TestBocMenuItem (IBusinessObject businessObject)
  {
    _businessObject = businessObject;
  }

  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }

  protected override void OnClick()
  {
    base.OnClick ();
    System.Diagnostics.Debug.WriteLine ("Clicked menu item '" + ItemID + "' for BusinessObject '" + _businessObject.ToString() + "'.");
  }

}

}
