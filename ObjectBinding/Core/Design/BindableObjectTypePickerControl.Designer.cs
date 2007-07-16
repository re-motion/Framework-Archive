using System.Windows.Forms;

namespace Rubicon.ObjectBinding.Design
{
  partial class BindableObjectTypePickerControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Label FilterLabel;
    private ListBox ClassList;
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
      this.ClassList = new System.Windows.Forms.ListBox ();
      this.SelectButton = new System.Windows.Forms.Button ();
      this.FilterField = new System.Windows.Forms.TextBox ();
      this.SuspendLayout ();
      // 
      // FilterLabel
      // 
      this.FilterLabel.AutoSize = true;
      this.FilterLabel.Location = new System.Drawing.Point (8, 11);
      this.FilterLabel.Name = "FilterLabel";
      this.FilterLabel.Size = new System.Drawing.Size (32, 13);
      this.FilterLabel.TabIndex = 6;
      this.FilterLabel.Text = "&Filter:";
      // 
      // ClassList
      // 
      this.ClassList.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ClassList.Location = new System.Drawing.Point (8, 36);
      this.ClassList.MultiColumn = true;
      this.ClassList.Name = "ClassList";
      this.ClassList.Size = new System.Drawing.Size (284, 225);
      this.ClassList.TabIndex = 9;
      // 
      // SelectButton
      // 
      this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SelectButton.Location = new System.Drawing.Point (8, 270);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size (64, 24);
      this.SelectButton.TabIndex = 8;
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
      this.FilterField.TabIndex = 7;
      this.FilterField.TextChanged += new System.EventHandler (this.FilterField_TextChanged);
      // 
      // BindableObjectTypePickerControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add (this.FilterLabel);
      this.Controls.Add (this.ClassList);
      this.Controls.Add (this.SelectButton);
      this.Controls.Add (this.FilterField);
      this.Name = "BindableObjectTypePickerControl";
      this.Size = new System.Drawing.Size (300, 301);
      this.Load += new System.EventHandler (this.BindableObjectTypePickerControl_Load);
      this.ResumeLayout (false);
      this.PerformLayout ();

    }

    #endregion
  }
}
