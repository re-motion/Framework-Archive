using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DateTimeProperty : NullableProperty, IBusinessObjectDateTimeProperty
{
  public DateTimeProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, isRequired, itemType, isList, isNullableType)
  {
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaDateTime.ToBoxedDateTime ((NaDateTime)internalValue);

    return base.FromInternalType (internalValue);
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue);

    return base.ToInternalType (publicValue);
  }
}
}
