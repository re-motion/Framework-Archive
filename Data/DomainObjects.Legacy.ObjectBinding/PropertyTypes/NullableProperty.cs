using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class NullableProperty: BaseProperty
  {
    private bool _isNullableType;
    private bool _isNaNullableType;

    public NullableProperty (
        IBusinessObjectClass businessObjectClass,
        PropertyInfo propertyInfo,
        bool isRequired,
        Type itemType,
        bool isList,
        bool isNullableType)
        : base (businessObjectClass, propertyInfo, isRequired, itemType, isList)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _isNullableType = isNullableType;
      _isNaNullableType = isNullableType && NaTypeUtility.IsNaNullableType (propertyInfo.PropertyType);
    }

    protected bool IsNullableType
    {
      get { return _isNullableType; }
    }

    protected bool IsNaNullableType
    {
      get { return _isNaNullableType; }
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      if (internalValue == null)
        return null;

      return base.FromInternalType (bindableObject, internalValue);
    }

    public override object ToInternalType (object publicValue)
    {
      if (!IsNullableType && publicValue == null)
        throw new InvalidNullAssignmentException (UnderlyingType);

      return publicValue;
    }
  }
}