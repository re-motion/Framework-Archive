using System;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{
/// <summary>
/// Summary description for ColumnDefinitionCollectionEditor.
/// </summary>
public class BocPropertyPathWrapperCollectionEditor: CollectionEditor
{
  public BocPropertyPathWrapperCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (BocPropertyPathWrapper)};
  }
}

}