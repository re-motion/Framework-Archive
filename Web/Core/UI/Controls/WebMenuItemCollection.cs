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

/// <summary> A collection of <see cref="MenuItem"/> objects. </summary>
[Editor (typeof (MenuItemCollectionEditor), typeof (UITypeEditor))]
public class MenuItemCollection: ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public MenuItemCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public MenuItemCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (MenuItem)})
  {
  }

  public new MenuItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (MenuItem[]) arrayList.ToArray (typeof (MenuItem));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new MenuItem this[int index]
  {
    get { return (MenuItem) List[index]; }
    set { List[index] = value; }
  }

  public virtual MenuItem[] GroupMenuItems (bool generateSeparators)
  {
    //  <string category, ArrayList menuItems>
    NameObjectCollection groupedMenuItems = new NameObjectCollection();
    ArrayList categories = new ArrayList();

    foreach (MenuItem menuItem in InnerList)
    {
      string category = StringUtility.NullToEmpty (menuItem.Category);
      ArrayList menuItems;
      if (groupedMenuItems.Contains (category))
      {
        menuItems = (ArrayList) groupedMenuItems[category];
      }
      else
      {
        menuItems = new ArrayList();
        groupedMenuItems.Add (category, menuItems);
        categories.Add (category);
      }
      menuItems.Add (menuItem);
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
          arrayList.Add (MenuItem.GetSeparator());
      }
      arrayList.AddRange ((ArrayList) groupedMenuItems[category]);
    }
    return (MenuItem[]) arrayList.ToArray (typeof (MenuItem));
  }
}

}
