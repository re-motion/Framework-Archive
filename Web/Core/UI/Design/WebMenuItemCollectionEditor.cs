using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{
public class WebMenuItemCollectionEditor: AdvancedCollectionEditor
{
  public WebMenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (WebMenuItem)};
  }
}

}