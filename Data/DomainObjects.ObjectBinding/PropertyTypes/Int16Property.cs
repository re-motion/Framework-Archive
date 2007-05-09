using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class Int16Property: NullableProperty, IBusinessObjectInt32Property
  {
    public Int16Property (
        IBusinessObjectClass businessObjectClass,
        PropertyInfo propertyInfo,
        bool isRequired,
        Type itemType,
        bool isList,
        bool isNullableType)
      : base (businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType)
    {
    }

    public bool AllowNegative
    {
      get { return true; }
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      if (IsList)
        return internalValue;

      if (IsNaNullableType)
        return NaInt16.ToBoxedInt16 ((NaInt16) internalValue);

      if (IsNullableType && internalValue == null)
        return null;

      return base.FromInternalType (bindableObject, short.Parse (internalValue.ToString ()));
    }

    public override object ToInternalType (object publicValue)
    {
      if (IsList)
        return publicValue;

      if (IsNaNullableType)
      {
        if (publicValue != null)
          return NaInt16.Parse (publicValue.ToString());
        else
          return NaInt16.Null;
      }

      if (IsNullableType && publicValue == null)
        return null;

      return short.Parse (base.ToInternalType (publicValue).ToString());
    }
  }
}