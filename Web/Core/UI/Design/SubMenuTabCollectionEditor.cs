using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

public class SubMenuTabCollectionEditor: AdvancedCollectionEditor
{
  public SubMenuTabCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (SubMenuTab)};
  }
}

}