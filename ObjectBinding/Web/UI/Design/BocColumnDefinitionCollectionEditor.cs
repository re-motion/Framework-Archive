using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

public class BocColumnDefinitionCollectionEditor: AdvancedCollectionEditor
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