using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Rubicon.ObjectBinding.Design
{

public class PropertyPathPickerEditor: UITypeEditor
{
  private IWindowsFormsEditorService _editorService = null;

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value) 
  {
    if (context != null
        && context.Instance != null
        && provider != null) 
    {
      _editorService = (IWindowsFormsEditorService) provider.GetService (typeof (IWindowsFormsEditorService));

      if (_editorService != null)
      {
        IPropertyPathBinding binding = context.Instance as IPropertyPathBinding;
        if (binding == null)
          throw new InvalidOperationException ("Cannot use PropertyPathPickerEditor for objects other than IPropertyPathBinding.");

        PropertyPathPickerControl propertyPathPickerControl = new PropertyPathPickerControl (binding);

        propertyPathPickerControl.Value = (string) value;
        propertyPathPickerControl.EditorService = _editorService;
        _editorService.DropDownControl (propertyPathPickerControl);
        value = propertyPathPickerControl.Value;
      }
    }
    return value;
  }

  public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context) 
  {
    if (context != null && context.Instance != null) 
      return UITypeEditorEditStyle.DropDown;
    return base.GetEditStyle(context);
  }
}

}
