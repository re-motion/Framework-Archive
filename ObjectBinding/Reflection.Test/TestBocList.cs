using System;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{

public class TestBocList: BocList
{

  protected override WebMenuItem[] InitializeRowMenuItems(IBusinessObject businessObject, int listIndex)
  {
    WebMenuItem[] baseMenuItems = base.InitializeRowMenuItems (businessObject, listIndex);

    WebMenuItem[] menuItems = new WebMenuItem[3];
    WebMenuItem menuItem = new WebMenuItem();
    menuItem.ItemID = listIndex.ToString() + "_0";
    menuItem.Text = menuItem.ItemID;
    menuItems[0] = menuItem;

    menuItem = new TestBocMenuItem (businessObject);
    menuItem.ItemID = listIndex.ToString() + "_1";
    menuItem.Text = menuItem.ItemID;
    menuItems[1] = menuItem;

    menuItem = new WebMenuItem();
    menuItem.ItemID = listIndex.ToString() + "_2";
    menuItem.Text =  menuItem.ItemID;
    menuItems[2] = menuItem;

    return (WebMenuItem[]) ArrayUtility.Combine (baseMenuItems, menuItems);
  }

  protected override void PreRenderRowMenuItems(WebMenuItemCollection menuItems, IBusinessObject businessObject, int listIndex)
  {
    base.PreRenderRowMenuItems (menuItems, businessObject,  listIndex);
    if (listIndex == 1)
      ((WebMenuItem)menuItems[2]).IsVisible = false;
    else if (listIndex == 2)
      ((WebMenuItem)menuItems[2]).IsDisabled = true;

    // In case the menu item is a dumb menu item
    // Set Text and Icon
    // Set IsVisible
    // Set isDisabled
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
    // handle the click
  }

  protected override void PreRender()
  {
    base.PreRender ();
    // Set Text and Icon
  }

  protected override bool EvaluateDisabled()
  {
    return base.EvaluateDisabled ();
    // if (base.EvaluateDisabled ())
    //   return true;
    // else
    //   do your own stuff
  }

  protected override bool EvaluateVisible()
  {
    return base.EvaluateVisible ();
    // if (! base.EvaluateVisible ())
    //   return false;
    // else
    //   do your own stuff
  }


}

}
