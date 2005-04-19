using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Rubicon.ObjectBinding.Design
{

/// <summary>
///   Editor applied to the string property used to set the 
///   <see cref="BusinessObjectPropertyPath.Identifier">BusinessObjectPropertyPath.Identifier</see>.
/// </summary>
public class PropertyPathPickerEditor: UITypeEditor
{
  private IWindowsFormsEditorService _editorService = null;

  /// <summary>
  ///   Called by visual studio when the PropertyPathIdentifier of a PropertyPathBinding or a SimpleColumnDefinition is edited.
  /// </summary>
  /// <param name="context"> Contains the PropertyPathBinding or the SimpleColumDefinition in property Instance. </param>
  /// <param name="provider"> </param>
  /// <param name="value"> The PropertyPathIdentifier (string). </param>
  /// <returns></returns>
  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value) 
  {
    if (context != null
        && context.Instance != null
        && provider != null) 
    {
      _editorService = (IWindowsFormsEditorService) provider.GetService (typeof (IWindowsFormsEditorService));

      if (_editorService != null)
      {
        IBusinessObjectClassSource propertySource = context.Instance as IBusinessObjectClassSource;
        if (propertySource == null)
          throw new InvalidOperationException ("Cannot use PropertyPathPickerEditor for objects other than IBusinessObjectClassSource.");

        PropertyPathPickerControl propertyPathPickerControl = new PropertyPathPickerControl (propertySource);
        propertyPathPickerControl.Value = (string) value;
        propertyPathPickerControl.EditorService = _editorService;

        // show editor
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
