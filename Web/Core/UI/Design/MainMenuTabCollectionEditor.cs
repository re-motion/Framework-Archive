using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

public class MainMenuTabCollectionEditor: AdvancedCollectionEditor
{
  public MainMenuTabCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (MainMenuTab)};
  }
}

}