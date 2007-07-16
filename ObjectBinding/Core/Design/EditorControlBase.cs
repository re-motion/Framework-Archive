using System.Windows.Forms;
using System.Windows.Forms.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design
{
  public abstract class EditorControlBase : UserControl
  {
    private readonly IWindowsFormsEditorService _editorService;

    protected EditorControlBase (IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("editorService", editorService);
      _editorService = editorService;
    }

    public abstract string Value { get; set; }

    public IWindowsFormsEditorService EditorService
    {
      get { return _editorService; }
    }
  }
}