using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{
public class BocColumnDefinitionSetCollectionEditor: CollectionEditor
{
  public BocColumnDefinitionSetCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (BocColumnDefinitionSet)};
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    IServiceProvider collectionEditorServiceProvider = null;
    if (provider.GetType() != typeof (CollectionEditorServiceProvider))
      collectionEditorServiceProvider = new CollectionEditorServiceProvider (provider, 600, 400, 2);
    else
      collectionEditorServiceProvider = provider;
    return base.EditValue (context, collectionEditorServiceProvider, value);
  }
}

}