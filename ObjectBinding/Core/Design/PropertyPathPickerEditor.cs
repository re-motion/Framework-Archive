using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design
{
  /// <summary>
  ///   Editor applied to the string property used to set the 
  ///   <see cref="BusinessObjectPropertyPath.Identifier">BusinessObjectPropertyPath.Identifier</see>.
  /// </summary>
  public class PropertyPathPickerEditor : DropDownEditorBase
  {
    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("editorService", editorService);

      IBusinessObjectClassSource propertySource = context.Instance as IBusinessObjectClassSource;
      if (propertySource == null)
        throw new InvalidOperationException ("Cannot use PropertyPathPickerEditor for objects other than IBusinessObjectClassSource.");

      return new PropertyPathPickerControl (propertySource, editorService);
    }
  }
}