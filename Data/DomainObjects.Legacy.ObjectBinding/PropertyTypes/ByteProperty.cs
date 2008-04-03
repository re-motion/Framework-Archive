using System;
using System.Reflection;
using Remotion.NullableValueTypes;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class ByteProperty: NullableProperty, IBusinessObjectNumericProperty
  {
    public ByteProperty (
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
      get { return false; }
    }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public Type Type
    {
      get { return typeof (int); }
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      if (IsList)
        return internalValue;

      if (IsNaNullableType)
        return NaByte.ToBoxedByte ((NaByte) internalValue);

      if (IsNullableType && internalValue == null)
        return null;

      return base.FromInternalType (bindableObject, int.Parse (internalValue.ToString ()));
    }

    public override object ToInternalType (object publicValue)
    {
      if (IsList)
        return publicValue;

      if (IsNaNullableType)
      {
        if (publicValue != null)
          return NaByte.Parse (publicValue.ToString());
        else
          return NaByte.Null;
      }

      if (IsNullableType && publicValue == null)
        return null;

      return byte.Parse (base.ToInternalType (publicValue).ToString());
    }
  }
}