using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Design;

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

public class BocColumnDefinitionCollectionEditor: CollectionEditor
{
  public BocColumnDefinitionCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (BocSimpleColumnDefinition), 
      typeof (BocCompoundColumnDefinition),
      typeof (BocCommandColumnDefinition)};
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

public class BocSimpleColumnDefinitionCollectionEditor: BocColumnDefinitionCollectionEditor
{
  public BocSimpleColumnDefinitionCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (BocSimpleColumnDefinition)};
  }
}
}