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
public class DateProperty : NullableProperty, IBusinessObjectDateProperty
{
  public DateProperty (      
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
    {
      NaDateTime value = (NaDateTime) internalValue;
      return NaDateTime.ToBoxedDateTime (value.Date);
    }
    else if (!IsList && !IsNullableType)
    {
      return base.FromInternalType (internalValue);
    }
    else
    {
      return ((DateTime)internalValue).Date;
    }
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (!IsList && IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue).Date;
    else
      return ((DateTime)publicValue).Date;
  }
}
}
