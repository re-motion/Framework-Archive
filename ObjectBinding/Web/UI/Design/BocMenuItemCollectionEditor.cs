using System;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
{

public class BocMenuItemCollectionEditor: WebMenuItemCollectionEditor
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
