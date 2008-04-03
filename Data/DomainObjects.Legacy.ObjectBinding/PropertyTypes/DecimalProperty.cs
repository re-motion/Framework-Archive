using System;
using System.Reflection;
using Remotion.NullableValueTypes;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class DecimalProperty: NullableProperty, IBusinessObjectNumericProperty
  {
    public DecimalProperty (
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

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public Type Type
    {
      get { return typeof (double); }
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      if (IsList)
        return internalValue;

      if (IsNaNullableType)
        return NaDecimal.ToBoxedDecimal ((NaDecimal) internalValue);

      if (IsNullableType && internalValue == null)
        return null;

      return base.FromInternalType (bindableObject, decimal.Parse (internalValue.ToString ()));
    }

    public override object ToInternalType (object publicValue)
    {
      if (IsList)
        return publicValue;

      if (IsNaNullableType)
      {
        if (publicValue != null)
          return NaDecimal.Parse (publicValue.ToString());
        else
          return NaDecimal.Null;
      }

      if (IsNullableType && publicValue == null)
        return null;

      return decimal.Parse (base.ToInternalType (publicValue).ToString());
    }
  }
}