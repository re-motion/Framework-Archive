using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{
public class BocListDesigner: 
    ControlDesigner, 
    IServiceProvider,
    System.Windows.Forms.Design.IWindowsFormsEditorService
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
    
    BocListTypeDescriptorContext context = new BocListTypeDescriptorContext (this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    if (serviceType == typeof (System.Windows.Forms.Design.IWindowsFormsEditorService))
      return this;
    else
      return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }

  public void DropDownControl(Control control)
  {
    throw new NotSupportedException();
  }

  public void CloseDropDown()
  {
    throw new NotSupportedException();
  }

  public System.Windows.Forms.DialogResult ShowDialog(Form dialog)
  {
    // TODO: BocListDesigner: Move Splitter
    //  private dialog.propertygrid.gridview.MoveSplitterTo(int)
    // TODO: BocListDesigner: Add description panel
    //  doccomment might be property description, is disabled

    dialog.Size = new Size (800, 500);
    dialog.StartPosition = FormStartPosition.CenterParent;
    return dialog.ShowDialog();
  }
}

public class BocListTypeDescriptorContext : ITypeDescriptorContext
{
  private BocListDesigner _designer;
  private PropertyDescriptor _propertyDescriptor;

  public BocListTypeDescriptorContext (
      BocListDesigner designer, 
      PropertyDescriptor propertyDescriptor)
  {
    _designer = designer;
    _propertyDescriptor = propertyDescriptor;
  }

  private IComponentChangeService ComponentChangeService
  {
    get { return (IComponentChangeService) this.GetService (typeof (IComponentChangeService)); }
  }

  public object GetService (Type serviceType)
  {
    return ((IServiceProvider)_designer).GetService(serviceType);
  }

  public IContainer Container
  {
    get { return _designer.Component.Site.Container; }
  }

  public object Instance
  {
    get { return _designer.Component; }
  }

  public PropertyDescriptor PropertyDescriptor
  {
    get { return _propertyDescriptor; }
  }

  public void OnComponentChanged()
  {
    if (ComponentChangeService != null)
      ComponentChangeService.OnComponentChanged (Instance, PropertyDescriptor, null, null);
  }

  public bool OnComponentChanging()
  {
    if (ComponentChangeService != null)
    {
      try
      {
        ComponentChangeService.OnComponentChanging (Instance, PropertyDescriptor);
      }
      catch (CheckoutException e)
      {
        if (e == CheckoutException.Canceled)
          return false;
        throw e;
      }
    }
    return true;
  }
}

}
