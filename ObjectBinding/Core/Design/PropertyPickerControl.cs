using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Rubicon.ObjectBinding
{

public class PropertyPathPickerControl: System.Windows.Forms.UserControl
{
  private System.Windows.Forms.Label FilterLabel;
  private System.Windows.Forms.TextBox FilterField;
  private System.Windows.Forms.Button SelectButton;

  private IBusinessObjectBoundControl _control;
  private string _value;
  private IWindowsFormsEditorService _editorService;
  private System.Windows.Forms.ListBox PropertiesList;

	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.Container components = null;

	public PropertyPathPickerControl (IBusinessObjectBoundControl control)
	{
    if (control.DataSource == null)
      throw new InvalidOperationException ("Cannot use PropertyPathEditor for controls without DataSource.");

		// This call is required by the Windows.Forms Form Designer.
		InitializeComponent();

    _editorService = (IWindowsFormsEditorService) GetService (typeof (IWindowsFormsEditorService));
    _control = control;

    FillList();
  }

  private void FillList()
  {
    PropertiesList.Items.Clear();

    string filter = FilterField.Text.ToLower().Trim();
    IBusinessObjectProperty[] properties = _control.DataSource.BusinessObjectClass.GetProperties();

    foreach (IBusinessObjectProperty property in properties)
    {
      if (   filter.Length == 0 
          || (property.Identifier != null && property.Identifier.ToLower().IndexOf (filter) >= 0))
      {
        bool isSupportedPropertyType = true;
        if (_control.SupportedPropertyInterfaces != null)
        {
          isSupportedPropertyType = false;
          foreach (Type supportedInterface in _control.SupportedPropertyInterfaces)
          {
            if (supportedInterface.IsAssignableFrom (property.GetType()))
            {
              isSupportedPropertyType = true;
              break;
            }
          }
        }
        if (isSupportedPropertyType)
          PropertiesList.Items.Add (property.Identifier);
      }
    }

    PropertiesList.Sorted = true;
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
    this.PropertiesList = new System.Windows.Forms.ListBox();
    this.SuspendLayout();
    // 
    // FilterLabel
    // 
    this.FilterLabel.Location = new System.Drawing.Point(8, 8);
    this.FilterLabel.Name = "FilterLabel";
    this.FilterLabel.Size = new System.Drawing.Size(32, 23);
    this.FilterLabel.TabIndex = 1;
    this.FilterLabel.Text = "&Filter";
    // 
    // FilterField
    // 
    this.FilterField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
      | System.Windows.Forms.AnchorStyles.Right)));
    this.FilterField.Location = new System.Drawing.Point(48, 8);
    this.FilterField.Name = "FilterField";
    this.FilterField.Size = new System.Drawing.Size(248, 20);
    this.FilterField.TabIndex = 2;
    this.FilterField.Text = "";
    this.FilterField.TextChanged += new System.EventHandler(this.FilterField_TextChanged);
    // 
    // SelectButton
    // 
    this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
    this.SelectButton.Location = new System.Drawing.Point(8, 248);
    this.SelectButton.Name = "SelectButton";
    this.SelectButton.Size = new System.Drawing.Size(64, 24);
    this.SelectButton.TabIndex = 3;
    this.SelectButton.Text = "&Select";
    this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
    // 
    // PropertiesList
    // 
    this.PropertiesList.Location = new System.Drawing.Point(8, 32);
    this.PropertiesList.Name = "PropertiesList";
    this.PropertiesList.Size = new System.Drawing.Size(288, 199);
    this.PropertiesList.TabIndex = 5;
    // 
    // PropertyPathPicker
    // 
    this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(224)), ((System.Byte)(224)), ((System.Byte)(224)));
    this.Controls.Add(this.PropertiesList);
    this.Controls.Add(this.SelectButton);
    this.Controls.Add(this.FilterField);
    this.Controls.Add(this.FilterLabel);
    this.Name = "PropertyPathPicker";
    this.Size = new System.Drawing.Size(304, 272);
    this.Load += new System.EventHandler(this.PropertyPathPicker_Load);
    this.ResumeLayout(false);

  }
	#endregion

  private void SelectButton_Click(object sender, System.EventArgs e)
  {
    _value = (string) PropertiesList.SelectedItem;
    if (_editorService != null)
      _editorService.CloseDropDown ();
  }

  private void FilterField_TextChanged(object sender, System.EventArgs e)
  {
    FillList();
  }

  private void PropertyPathPicker_Load(object sender, System.EventArgs e)
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
        for (int i = 0; i < PropertiesList.Items.Count; ++i)
        {
          string item = (string) PropertiesList.Items[i];
          if (item == value)
            PropertiesList.SelectedIndex = i;
        }
      }
    }
  }
}

}
