using System;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;


namespace Rubicon.Data.DomainObjects.ObjectBinding.Design
{

public class ClassPickerEditor: UITypeEditor
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
        ClassPickerControl classPickerControl = new ClassPickerControl ();

        classPickerControl.Value = (string) value;
        classPickerControl.EditorService = _editorService;
        _editorService.DropDownControl (classPickerControl);
        value = classPickerControl.Value;
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
