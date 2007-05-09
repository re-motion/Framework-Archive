using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class SingleProperty: NullableProperty, IBusinessObjectDoubleProperty
  {
    public SingleProperty (
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
        return NaSingle.ToBoxedSingle ((NaSingle) internalValue);

      if (IsNullableType && internalValue == null)
        return null;

      return base.FromInternalType (bindableObject, float.Parse (internalValue.ToString ()));
    }

    public override object ToInternalType (object publicValue)
    {
      if (IsList)
        return publicValue;

      if (IsNaNullableType)
      {
        if (publicValue != null)
          return NaSingle.Parse (publicValue.ToString());
        else
          return NaSingle.Null;
      }

      if (IsNullableType && publicValue == null)
        return null;

      return float.Parse (base.ToInternalType (publicValue).ToString());
    }
  }
}