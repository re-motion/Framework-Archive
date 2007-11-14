using System;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: Doc
  public class BindableObjectDataSource : BusinessObjectDataSource
  {
    private IBusinessObject _businessObject;
    private string _typeName;

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
    [DefaultValue (null)]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    [TypeConverter (typeof (TypeNameConverter))]
    public Type Type
    {
      get
      {
        if (_typeName == null)
          return null;

        if (IsDesignMode)
          return TypeUtility.GetDesignModeType (_typeName, Site, false);

        return TypeUtility.GetType (_typeName, true, false);
      }
      set
      {
        if (value == null)
          _typeName = null;
        else
          _typeName = TypeUtility.GetPartialAssemblyQualifiedName (value);
      }
    }

    private IBusinessObjectClass GetBusinessObjectClass ()
    {
      if (Type == null)
        return null;

      return BindableObjectProvider.Current.GetBindableObjectClass (Type);
    }

    private bool IsDesignMode
    {
      get { return Site != null && Site.DesignMode; }
    }
  }
}