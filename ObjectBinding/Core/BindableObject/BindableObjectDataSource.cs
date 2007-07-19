using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: Doc
  public class BindableObjectDataSource : BusinessObjectDataSource
  {
    private IBusinessObject _businessObject;
    private string _typeName = string.Empty;

    public BindableObjectDataSource ()
    {
    }

    public override IBusinessObject BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    public override IBusinessObjectClass BusinessObjectClass
    {
      get { return GetBusinessObjectClass(); }
    }

    [Category ("Data")]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    public string TypeName
    {
      get { return _typeName; }
      set { _typeName = StringUtility.NullToEmpty (value); }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public Type Type
    {
      get
      {
        if (string.IsNullOrEmpty (_typeName))
          return null;

        if (IsDesignMode)
          return TypeUtility.GetDesignModeType (_typeName, Site, false);

        return TypeUtility.GetType (_typeName, true, false);
      }
    }

    private IBusinessObjectClass GetBusinessObjectClass ()
    {
      if (Type == null)
        return null;

      if (IsDesignMode)
      {
        BindableObjectProvider designModeProvider = BindableObjectProvider.CreateDesignModeBindableObjectProvider();
        return designModeProvider.GetBindableObjectClass (Type);
      }

      return BindableObjectProvider.Current.GetBindableObjectClass (Type);
    }

    private bool IsDesignMode
    {
      get { return Site != null && Site.DesignMode; }
    }
  }
}