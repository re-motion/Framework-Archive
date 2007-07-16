using System;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.Design
{
  public partial class BindableObjectTypePickerControl : EditorControlBase
  {
    private string _value;

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

    private void SelectButton_Click (object sender, EventArgs e)
    {
    }

    private void FilterField_TextChanged (object sender, EventArgs e)
    {
    }

    private void FillList ()
    {
      ClassList.BeginUpdate();
      ClassList.Items.Clear();

      string filterText = FilterField.Text.ToLower().Trim();

      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) ServiceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService != null)
      {
        foreach (Type type in typeDiscoveryService.GetTypes (typeof (object), false))
        {
          if (MixinConfiguration.ActiveContext.ContainsClassContext (type.IsGenericType ? type.GetGenericTypeDefinition() : type)
              && TypeFactory.GetActiveConfiguration (type).Mixins.ContainsKey (typeof (BindableObjectMixin)))
          {
            ClassList.Items.Add (type.FullName);
          }
        }
      }

      ClassList.Sorted = true;
      ClassList.EndUpdate();
    }

    private void BindableObjectTypePickerControl_Load (object sender, EventArgs e)
    {
      FillList();
    }
  }
}