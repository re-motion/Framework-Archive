using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
{
public class BocListDesigner: WebControlDesigner, IServiceProvider
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

    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    BocColumnDefinitionCollectionEditor editor = null;
    editor = (BocColumnDefinitionCollectionEditor) TypeDescriptor.GetEditor (value, typeof(UITypeEditor));
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
