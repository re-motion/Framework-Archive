using System;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{
public class BusinessObjectPropertyPathBindingCollectionEditor: CollectionEditor
{
  public BusinessObjectPropertyPathBindingCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (BusinessObjectPropertyPathBinding)};
  }
}

}