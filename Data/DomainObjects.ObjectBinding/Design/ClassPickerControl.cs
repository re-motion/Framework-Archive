using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Design
{

public class ClassPickerControl: System.Windows.Forms.UserControl
{
  private System.Windows.Forms.Label FilterLabel;
  private System.Windows.Forms.TextBox FilterField;
  private System.Windows.Forms.Button SelectButton;

  private string _value;
  private IWindowsFormsEditorService _editorService;
  private System.Windows.Forms.ListBox ClassList;

	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.Container components = null;

	public ClassPickerControl ()
	{
 		// This call is required by the Windows.Forms Form Designer.
		InitializeComponent();

    _editorService = (IWindowsFormsEditorService) GetService (typeof (IWindowsFormsEditorService));

    FillList();
  }

  private void FillList()
  {
    ClassList.Items.Clear();

    string filter = FilterField.Text.ToLower().Trim();

    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      if (classDefinition.ClassType.IsSubclassOf (typeof (BindableDomainObject)))
      {
        if (filter == string.Empty
            || (classDefinition.ID.ToLower().IndexOf (filter) >= 0))
        {
          ClassList.Items.Add (classDefinition.ID);
        }
      }
    }

    ClassList.Sorted = true;
	}

	/// <summary> 
	/// Clean up any resources being used.
	/// </summary>
	protected override void Dispose( bool disposing )
	{
		if( disposing )
		{
			if(components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose( disposing );
	}

	#region Component Designer generated code
	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
    this.FilterLabel = new System.Windows.Forms.Label();
    this.FilterField = new System.Windows.Forms.TextBox();
    this.SelectButton = new System.Windows.Forms.Button();
    this.ClassList = new System.Windows.Forms.ListBox();
    this.SuspendLayout();
    // 
    // FilterLabel
    // 
    this.FilterLabel.AutoSize = true;
    this.FilterLabel.Location = new System.Drawing.Point(8, 11);
    this.FilterLabel.Name = "FilterLabel";
    this.FilterLabel.Size = new System.Drawing.Size(33, 16);
    this.FilterLabel.TabIndex = 1;
    this.FilterLabel.Text = "&Filter:";
    // 
    // FilterField
    // 
    this.FilterField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
      | System.Windows.Forms.AnchorStyles.Right)));
    this.FilterField.Location = new System.Drawing.Point(49, 8);
    this.FilterField.Name = "FilterField";
    this.FilterField.Size = new System.Drawing.Size(243, 20);
    this.FilterField.TabIndex = 2;
    this.FilterField.Text = "";
    this.FilterField.TextChanged += new System.EventHandler(this.FilterField_TextChanged);
    // 
    // SelectButton
    // 
    this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
    this.SelectButton.Location = new System.Drawing.Point(8, 239);
    this.SelectButton.Name = "SelectButton";
    this.SelectButton.Size = new System.Drawing.Size(64, 24);
    this.SelectButton.TabIndex = 3;
    this.SelectButton.Text = "&Select";
    this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
    // 
    // ClassList
    // 
    this.ClassList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
      | System.Windows.Forms.AnchorStyles.Left) 
      | System.Windows.Forms.AnchorStyles.Right)));
    this.ClassList.Location = new System.Drawing.Point(8, 32);
    this.ClassList.Name = "ClassList";
    this.ClassList.Size = new System.Drawing.Size(284, 199);
    this.ClassList.TabIndex = 5;
    // 
    // ClassPickerControl
    // 
    this.BackColor = System.Drawing.SystemColors.Control;
    this.Controls.Add(this.FilterLabel);
    this.Controls.Add(this.ClassList);
    this.Controls.Add(this.SelectButton);
    this.Controls.Add(this.FilterField);
    this.Name = "ClassPickerControl";
    this.Size = new System.Drawing.Size(300, 271);
    this.Load += new System.EventHandler(this.ClassPathPicker_Load);
    this.ResumeLayout(false);

  }
	#endregion

  private void SelectButton_Click(object sender, System.EventArgs e)
  {
    if (ClassList.SelectedItem != null)
    {
      Type classType = MappingConfiguration.Current.ClassDefinitions[ClassList.SelectedItem.ToString ()].ClassType;
      _value = string.Format ("{0}, {1}", classType.FullName, classType.Assembly.GetName().Name);
    }

    if (_editorService != null)
      _editorService.CloseDropDown ();
  }

  private void FilterField_TextChanged(object sender, System.EventArgs e)
  {
    FillList();
  }

  private void ClassPathPicker_Load(object sender, System.EventArgs e)
  {
  
  }

  public IWindowsFormsEditorService EditorService
  {
    get { return _editorService; }
    set { _editorService = value; }
  }

  public string Value 
  {
    get 
    { 
      return _value;
    }

    set
    {
      if (value == null)
        _value = string.Empty;
      else
        _value = value.Trim();

      if (_value.Length > 0)
      {
        for (int i = 0; i < ClassList.Items.Count; ++i)
        {
          string item = (string) ClassList.Items[i];
          if (item == value)
            ClassList.SelectedIndex = i;
        }
      }
    }
  }
}

}
