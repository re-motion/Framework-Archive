using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Design;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A collection of <see cref="WebMenuItem"/> objects. </summary>
[Editor (typeof (MenuItemCollectionEditor), typeof (UITypeEditor))]
public class WebMenuItemCollection: ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public WebMenuItemCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public WebMenuItemCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (WebMenuItem)})
  {
  }

  public new WebMenuItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (WebMenuItem[]) arrayList.ToArray (typeof (WebMenuItem));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new WebMenuItem this[int index]
  {
    get { return (WebMenuItem) List[index]; }
    set { List[index] = value; }
  }

  public static WebMenuItem[] GroupMenuItems (WebMenuItem[] menuItems, bool generateSeparators)
  {
    //  <string category, ArrayList menuItems>
    NameObjectCollection groupedMenuItems = new NameObjectCollection();
    ArrayList categories = new ArrayList();
    
    foreach (WebMenuItem menuItem in menuItems)
    {
      string category = StringUtility.NullToEmpty (menuItem.Category);
      ArrayList menuItemsForCategory;
      if (groupedMenuItems.Contains (category))
      {
        menuItemsForCategory = (ArrayList) groupedMenuItems[category];
      }
      else
      {
        menuItemsForCategory = new ArrayList();
        groupedMenuItems.Add (category, menuItemsForCategory);
        categories.Add (category);
      }
      menuItemsForCategory.Add (menuItem);
    }
      
    ArrayList arrayList = new ArrayList();
    bool isFirst = true;
    foreach (string category in categories)
    {
      if (generateSeparators)
      {
        if (isFirst)
          isFirst = false;
        else
          arrayList.Add (WebMenuItem.GetSeparator());
      }
      arrayList.AddRange ((ArrayList) groupedMenuItems[category]);
    }
    return (WebMenuItem[]) arrayList.ToArray (typeof (WebMenuItem));
  }

  public WebMenuItem[] GroupMenuItems (bool generateSeparators)
  {
    return WebMenuItemCollection.GroupMenuItems (ToArray(), generateSeparators);
  }
}

}
