using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DateProperty : NullableProperty, IBusinessObjectDateProperty
{
  public DateProperty (      
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, isRequired, itemType, isList, isNullableType)
  {
  }

  public override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
    {
      NaDateTime value = (NaDateTime) internalValue;
      return NaDateTime.ToBoxedDateTime (value.Date);
    }

    return base.FromInternalType (internalValue);
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue).Date;

    return base.ToInternalType (publicValue);
  }
}
}
