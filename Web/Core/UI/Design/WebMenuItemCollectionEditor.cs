using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{
public class MenuItemCollectionEditor: CollectionEditor
{
  public MenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (MenuItem)};
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    IServiceProvider collectionEditorServiceProvider = null;
    if (provider.GetType() != typeof (CollectionEditorServiceProvider))
      collectionEditorServiceProvider = new CollectionEditorServiceProvider (provider, 800, 500, 4);
    else
      collectionEditorServiceProvider = provider;
    return base.EditValue (context, collectionEditorServiceProvider, value);
  }
}

}