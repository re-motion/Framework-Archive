using System;
using Rubicon.Web.UI.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{

public class BocMenuItemCollectionEditor: MenuItemCollectionEditor
{
  public BocMenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (BocMenuItem)};
  }
}

}
