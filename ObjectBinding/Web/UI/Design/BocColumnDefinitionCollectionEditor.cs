using System;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{
/// <summary>
/// Summary description for ColumnDefinitionCollectionEditor.
/// </summary>
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
}
}