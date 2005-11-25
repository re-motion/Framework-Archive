using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

public class TabStripSubMenuItemCollectionEditor: AdvancedCollectionEditor
{
  public TabStripSubMenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (TabStripSubMenuItem)};
  }
}

}