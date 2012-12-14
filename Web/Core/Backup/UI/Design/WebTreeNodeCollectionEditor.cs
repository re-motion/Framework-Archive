using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

public class WebTreeNodeCollectionEditor: AdvancedCollectionEditor
{
  public WebTreeNodeCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (WebTreeNode)};
  }
}

}