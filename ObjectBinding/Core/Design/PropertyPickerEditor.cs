using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;

using Rubicon.CooNet.Web.Controls;

namespace Rubicon.ObjectBinding
{

public class PropertyPathEditor: UITypeEditor
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
        CnObject objectClass = null;
        IFscObjectBoundControl source = context.Instance as IFscObjectBoundControl;
        if (source != null && source.FscObject != null)
          objectClass = source.FscObject.ObjectClass;

        PropertyPathPicker pathPicker = new PropertyPathPicker (objectClass);
        pathPicker.Value = (string) value;
        pathPicker.EditorService = _editorService;
        _editorService.DropDownControl (pathPicker);
        value = pathPicker.Value;
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
