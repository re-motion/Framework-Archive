using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Design
{
public class BocListDesigner: ControlDesigner, IServiceProvider
{
  private DesignerVerbCollection _verbs = null;

	public BocListDesigner()
	{
    _verbs = new DesignerVerbCollection();
    _verbs.Add (new DesignerVerb ("Edit Fixed Columns", new EventHandler(OnVerbEditFixedColumns)));
  }

  private void OnVerbEditFixedColumns (object sender, EventArgs e) 
  {
    BocList bocList = Component as BocList;
    if (bocList == null)
      throw new InvalidOperationException ("Cannot use BocListDesigner for objects other than BocList.");

    PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties (bocList);
    PropertyDescriptor propertyDescriptor = propertyDescriptors["FixedColumns"];

    BocColumnDefinitionCollectionEditor editor = null;
    editor = (BocColumnDefinitionCollectionEditor) TypeDescriptor.GetEditor (
        propertyDescriptor.PropertyType, 
        typeof(UITypeEditor));
    
    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }
}

}
