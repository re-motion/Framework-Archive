using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design.BindableObject
{
  public partial class BindableObjectTypePickerControl : EditorControlBase
  {
    private static bool s_isGacIncluded;

    private string _value;
    private TypeTreeViewController _typeTreeViewController;
    private SearchFieldController _searchFieldController;

    public BindableObjectTypePickerControl (IServiceProvider provider, IWindowsFormsEditorService editorService)
        : base (provider, editorService)
    {
      InitializeComponent();
    }

    public BindableObjectTypePickerControl ()
    {
      InitializeComponent();
    }

    public override string Value
    {
      get { return _value; }
      set { _value = value; }
    }

    private void BindableObjectTypePickerControl_Load (object sender, EventArgs e)
    {
      IncludeGacCheckBox.Checked = s_isGacIncluded;
      PopulateTypeTreeView();
      _searchFieldController = new SearchFieldController (SearchField, SearchButton);
    }

    private void IncludeGacCheckBox_CheckedChanged (object sender, System.EventArgs e)
    {
      s_isGacIncluded = IncludeGacCheckBox.Checked;
      PopulateTypeTreeView ();
    }

    private void TypeTreeView_AfterSelect (object sender, TreeViewEventArgs e)
    {
      HandleSelectionChanged();
    }

    private void PopulateTypeTreeView ()
    {
      Cursor cursorBackUp = Cursor;
      try
      {
        Cursor = Cursors.WaitCursor;
        BindableObjectTypeFinder typeFinder = new BindableObjectTypeFinder (ServiceProvider);

        _typeTreeViewController = new TypeTreeViewController (typeFinder.GetTypes(), TypeTreeView);
        _typeTreeViewController.PopulateTreeNodes (_value);
      }
      finally
      {
        Cursor = cursorBackUp;
      }
    }

    private void HandleSelectionChanged ()
    {
      Type type = _typeTreeViewController.GetSelectedType();
      if (type != null)
        SelectButton.Enabled = true;
      else
        SelectButton.Enabled = false;
    }

    private void SelectButton_Click (object sender, EventArgs e)
    {
      Type type = _typeTreeViewController.GetSelectedType();
      if (type != null)
        _value = TypeUtility.GetPartialAssemblyQualifiedName (type);

      EditorService.CloseDropDown();
    }

    private void FilterField_TextChanged (object sender, EventArgs e)
    {
    }
  }
}