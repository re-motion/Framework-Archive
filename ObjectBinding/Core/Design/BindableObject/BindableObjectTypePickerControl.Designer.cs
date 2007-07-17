using System.Windows.Forms;

namespace Rubicon.ObjectBinding.Design.BindableObject
{
  partial class BindableObjectTypePickerControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Label FilterLabel;
    private Button SelectButton;
    private TextBox FilterField;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose ();
      }
      base.Dispose (disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.FilterLabel = new System.Windows.Forms.Label ();
      this.SelectButton = new System.Windows.Forms.Button ();
      this.FilterField = new System.Windows.Forms.TextBox ();
      this.IncludeGacCheckBox = new System.Windows.Forms.CheckBox ();
      this.TypeTreeView = new System.Windows.Forms.TreeView ();
      this.SuspendLayout ();
      // 
      // FilterLabel
      // 
      this.FilterLabel.AutoSize = true;
      this.FilterLabel.Location = new System.Drawing.Point (9, 11);
      this.FilterLabel.Name = "FilterLabel";
      this.FilterLabel.Size = new System.Drawing.Size (32, 13);
      this.FilterLabel.TabIndex = 1;
      this.FilterLabel.Text = "&Filter:";
      // 
      // SelectButton
      // 
      this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SelectButton.Location = new System.Drawing.Point (8, 270);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size (64, 24);
      this.SelectButton.TabIndex = 3;
      this.SelectButton.Text = "&Select";
      this.SelectButton.Click += new System.EventHandler (this.SelectButton_Click);
      // 
      // FilterField
      // 
      this.FilterField.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.FilterField.Location = new System.Drawing.Point (48, 8);
      this.FilterField.Name = "FilterField";
      this.FilterField.Size = new System.Drawing.Size (244, 20);
      this.FilterField.TabIndex = 1;
      this.FilterField.TextChanged += new System.EventHandler (this.FilterField_TextChanged);
      // 
      // IncludeGacCheckBox
      // 
      this.IncludeGacCheckBox.AutoSize = true;
      this.IncludeGacCheckBox.Location = new System.Drawing.Point (154, 275);
      this.IncludeGacCheckBox.Name = "IncludeGacCheckBox";
      this.IncludeGacCheckBox.Size = new System.Drawing.Size (118, 17);
      this.IncludeGacCheckBox.TabIndex = 4;
      this.IncludeGacCheckBox.Text = "Include &GAC Assemblies";
      this.IncludeGacCheckBox.UseVisualStyleBackColor = true;
      this.IncludeGacCheckBox.CheckedChanged += new System.EventHandler (IncludeGacCheckBox_CheckedChanged);
      // 
      // TypeTreeView
      // 
      this.TypeTreeView.HideSelection = false;
      this.TypeTreeView.Location = new System.Drawing.Point (8, 36);
      this.TypeTreeView.Name = "TypeTreeView";
      this.TypeTreeView.ShowNodeToolTips = true;
      this.TypeTreeView.Size = new System.Drawing.Size (284, 225);
      this.TypeTreeView.Sorted = true;
      this.TypeTreeView.TabIndex = 2;
      this.TypeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler (this.TypeTreeView_AfterSelect);
      // 
      // BindableObjectTypePickerControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add (this.IncludeGacCheckBox);
      this.Controls.Add (this.FilterLabel);
      this.Controls.Add (this.SelectButton);
      this.Controls.Add (this.FilterField);
      this.Controls.Add (this.TypeTreeView);
      this.Name = "BindableObjectTypePickerControl";
      this.Size = new System.Drawing.Size (300, 301);
      this.Load += new System.EventHandler (this.BindableObjectTypePickerControl_Load);
      this.ResumeLayout (false);
      this.PerformLayout ();

    }

    #endregion

    private CheckBox IncludeGacCheckBox;
    private TreeView TypeTreeView;
  }
}