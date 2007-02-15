using System;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
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
      typeof (BocCommandColumnDefinition),
      typeof (BocCompoundColumnDefinition),
      typeof (BocRowEditModeColumnDefinition),
      typeof (BocCustomColumnDefinition),
      typeof (BocDropDownMenuColumnDefinition),
      typeof (BocAllPropertiesPlacehoderColumnDefinition)};
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