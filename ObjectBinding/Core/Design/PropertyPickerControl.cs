using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Rubicon.ObjectBinding
{
/// <summary>
/// Summary description for PropertyPathPicker.
/// </summary>
public class PropertyPathPicker : System.Windows.Forms.UserControl
{
  private System.Windows.Forms.TreeView PathTree;
  private System.Windows.Forms.Label FilterLabel;
  private System.Windows.Forms.TextBox FilterField;
  private System.Windows.Forms.Button SelectButton;

  private CnObject _objectClass;
  private string _value;
  private IWindowsFormsEditorService _editorService;
  private System.Windows.Forms.CheckBox ClassFilterCheck;

	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.Container components = null;

	public PropertyPathPicker (CnObject objectClass)
	{
		// This call is required by the Windows.Forms Form Designer.
		InitializeComponent();

    ClassFilterCheck.Text = string.Format (ClassFilterCheck.Text, objectClass.Name);

    _editorService = (IWindowsFormsEditorService) GetService (typeof (IWindowsFormsEditorService));

    if (objectClass == null)
      _objectClass = Classes.Object;
    else
      _objectClass = objectClass;

    FillTree();
  }

  private void FillTree()
  {
    PathTree.Nodes.Clear();
    string filter = FilterField.Text.ToLower().Trim();
    CnObject[] properties;
    if (ClassFilterCheck.Checked)
    {
      properties = (CnObject[]) _objectClass.GetAttribute (Attributes.classallattributes);
    }
    else
    {
      QueryBuilder query = new QueryBuilder (Classes.AttributeDefinition);
      query.Limit = 200;
      if (filter.Length > 0)
        query.WhereClause.AddCondition (new QueryCompareCondition (".reference", QueryCompareOperator.Like, "%" + filter + "%"));
      properties = CnRuntime.Current.SearchObjects (query.ToString());

//      properties = CnRuntime.Current.SearchObjects ("SELECT * FROM AttributeDefinition where reference LIKE \"%" + Query.QueryBuilder.To
//      CnObject[] searchResult = CnRuntime.Current.SearchLocalObjects ("AttributeDefinition");
//      const int maxResult = 200;
//      if (searchResult.Length > maxResult)
//      {
//        properties = new CnObject[maxResult];
//        Array.Copy (searchResult, 0, properties, 0, maxResult);
//      }
//      else
//      {
//        properties = searchResult;
//      }
    }

    foreach (CnObject property in properties)
    {
      if (   filter.Length == 0 
          || (property.Reference != null && property.Reference.ToLower().IndexOf (filter) >= 0))
      {
        TreeNode node = new TreeNode (property.Reference);
        AddChildren (node, property);

			  PathTree.Nodes.Add (node);
      }
    }

    PathTree.PathSeparator = "~";
    PathTree.Sorted = true;
	}

  private void AddChildren (TreeNode node, CnObject property)
  {
    CnObject type = (CnObject) property.GetAttributeValue (Attributes.attrtype);
    if (type == null)
      return;

    CnObject typeClass = type.Class;
    if (typeClass == Classes.TypeObjectDef)
    {
      // TODO
    }
    else if (typeClass == Classes.TypeAggregateDef)
    {
      foreach (CnObject compProperty in type.GetAttribute ("typecompattrs"))
      {
        TreeNode childNode = new TreeNode (compProperty.Reference);
        AddChildren (childNode, compProperty);
        node.Nodes.Add (childNode);
      }
    }
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
    this.PathTree = new System.Windows.Forms.TreeView();
    this.FilterLabel = new System.Windows.Forms.Label();
    this.FilterField = new System.Windows.Forms.TextBox();
    this.SelectButton = new System.Windows.Forms.Button();
    this.ClassFilterCheck = new System.Windows.Forms.CheckBox();
    this.SuspendLayout();
    // 
    // PathTree
    // 
    this.PathTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
      | System.Windows.Forms.AnchorStyles.Left) 
      | System.Windows.Forms.AnchorStyles.Right)));
    this.PathTree.ImageIndex = -1;
    this.PathTree.Location = new System.Drawing.Point(0, 32);
    this.PathTree.Name = "PathTree";
    this.PathTree.SelectedImageIndex = -1;
    this.PathTree.Size = new System.Drawing.Size(304, 208);
    this.PathTree.TabIndex = 0;
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
    // ClassFilterCheck
    // 
    this.ClassFilterCheck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
      | System.Windows.Forms.AnchorStyles.Right)));
    this.ClassFilterCheck.Checked = true;
    this.ClassFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
    this.ClassFilterCheck.Location = new System.Drawing.Point(88, 240);
    this.ClassFilterCheck.Name = "ClassFilterCheck";
    this.ClassFilterCheck.Size = new System.Drawing.Size(208, 32);
    this.ClassFilterCheck.TabIndex = 4;
    this.ClassFilterCheck.Text = "&Properties of class {0} only";
    this.ClassFilterCheck.CheckedChanged += new System.EventHandler(this.ClassFilterCheck_CheckedChanged);
    // 
    // PropertyPathPicker
    // 
    this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(224)), ((System.Byte)(224)), ((System.Byte)(224)));
    this.Controls.Add(this.ClassFilterCheck);
    this.Controls.Add(this.SelectButton);
    this.Controls.Add(this.FilterField);
    this.Controls.Add(this.FilterLabel);
    this.Controls.Add(this.PathTree);
    this.Name = "PropertyPathPicker";
    this.Size = new System.Drawing.Size(304, 272);
    this.ResumeLayout(false);

  }
	#endregion

  private void SelectButton_Click(object sender, System.EventArgs e)
  {
    if (PathTree.SelectedNode != null)
    {
      string path = PathTree.SelectedNode.FullPath;
      _value = path.Replace ("@", "_").Replace (":", "_").Replace (".", "_").Replace ("~", ".");
      if (_editorService != null)
        _editorService.CloseDropDown ();
    }
  }

  private void FilterField_TextChanged(object sender, System.EventArgs e)
  {
    FillTree();
  }

  private void ClassFilterCheck_CheckedChanged(object sender, System.EventArgs e)
  {
    FillTree();
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

      if (value.Length > 0)
      {
        CnPropertyPath path = CnPropertyPath.Parse (value);
        TreeNodeCollection nodes = PathTree.Nodes;
        foreach (CnPropertyPath.Element element in path)
        {
          if (nodes == null)
            break;
          TreeNode node = null;
          foreach (TreeNode childNode in nodes)
          {
            if (childNode.Text == element.Property.Reference)
            {
              node = childNode;
              break;
            }
          }
          if (node == null)
            break;
          PathTree.SelectedNode = node;
          nodes = node.Nodes;
        }
      }
    }
  }
}

}
