using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DateTimeProperty : NullableProperty, IBusinessObjectDateTimeProperty
{
  public DateTimeProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, propertyDefinition, itemType, isList, isNullableType)
  {
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (!IsList && IsNullableType)
      return NaDateTime.ToBoxedDateTime ((NaDateTime)internalValue);
    else if (!IsList && !IsNullableType)
      return base.FromInternalType (internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (!IsList && IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue);
    else
      return publicValue;
  }
}
}
