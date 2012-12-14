using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.Design
{
public class PropertyPathBindingCollectionEditor: AdvancedCollectionEditor
{
  public PropertyPathBindingCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (PropertyPathBinding)};
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    return EditValue (context, provider, value, 600, 400, 2);
  }
}

}