using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class DateTimeProperty: NullableProperty, IBusinessObjectDateTimeProperty
  {
    public DateTimeProperty (
        IBusinessObjectClass businessObjectClass,
        PropertyInfo propertyInfo,
        bool isRequired,
        Type itemType,
        bool isList,
        bool isNullableType)
      : base (businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType)
    {
    }

    public DateTimeType Type
    {
      get { return DateTimeType.DateTime; }
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      if (IsList)
        return internalValue;

      if (IsNaNullableType)
        return NaDateTime.ToBoxedDateTime ((NaDateTime) internalValue);

      return base.FromInternalType (bindableObject, internalValue);
    }

    public override object ToInternalType (object publicValue)
    {
      if (IsList)
        return publicValue;

      if (IsNaNullableType)
        return NaDateTime.FromBoxedDateTime (publicValue);

      return base.ToInternalType (publicValue);
    }
  }
}