using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.UnitTests.Design
{
  public abstract class MockDropDownEditorBase : DropDownEditorBase
  {
    public abstract EditorControlBase NewCreateEditorControl (ITypeDescriptorContext context, IWindowsFormsEditorService editorService);

    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
    {
      return NewCreateEditorControl (context, editorService);
    }
  }
}