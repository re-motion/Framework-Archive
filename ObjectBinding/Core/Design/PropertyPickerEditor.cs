using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Rubicon.ObjectBinding.Design
{

/// <summary>
///   Editor applied to an 
///   <see cref="IBusinessObjectBoundControl.PropertyIdentifier">IBusinessObjectBoundControl.PropertyIdentifier</see>.
/// </summary>
public class PropertyPickerEditor: UITypeEditor
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
        IBusinessObjectBoundControl control = context.Instance as IBusinessObjectBoundControl;
        if (control == null)
          throw new InvalidOperationException ("Cannot use PropertyPickerEditor for objects other than IBusinessObjectBoundControl.");

        PropertyPickerControl pathPickerControl = new PropertyPickerControl (control);

        pathPickerControl.Value = (string) value;
        pathPickerControl.EditorService = _editorService;
        _editorService.DropDownControl (pathPickerControl);
        value = pathPickerControl.Value;
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
