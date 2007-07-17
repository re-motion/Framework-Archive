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
          return GetDesignModeType();

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

    private Type GetDesignModeType ()
    {
      IDesignerHost designerHost = (IDesignerHost) Site.GetService (typeof (IDesignerHost));
      Assertion.Assert (designerHost != null, "No IDesignerHost found.");
      Type type = designerHost.GetType (TypeUtility.ParseAbbreviatedTypeName (_typeName));
      if (type == null)
        throw new TypeLoadException (string.Format ("Could not load type '{0}'.", TypeUtility.ParseAbbreviatedTypeName (_typeName)));
      return type;
    }
  }
}