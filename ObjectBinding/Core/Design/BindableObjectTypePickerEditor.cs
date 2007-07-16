using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design
{
  //TODO: doc
  public class BindableObjectTypePickerEditor : DropDownEditorBase
  {
    public BindableObjectTypePickerEditor ()
    {
    }

    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);


      return new BindableObjectTypePickerControl (provider, editorService);
    }
  }
}