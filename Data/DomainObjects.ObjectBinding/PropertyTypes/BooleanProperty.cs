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
public class BooleanProperty : NullableProperty, IBusinessObjectBooleanProperty, IBusinessObjectEnumerationProperty
{
  public BooleanProperty (      
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
    if (!IsList && IsNullableType)
      return NaBoolean.ToBoxedBoolean ((NaBoolean)internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (!IsList && IsNullableType)
      return NaBoolean.FromBoxedBoolean (publicValue);
    else
      return publicValue;

  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return BooleanToEnumPropertyConverter.GetValues ();
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    return BooleanToEnumPropertyConverter.GetValues ();
  }

  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    return BooleanToEnumPropertyConverter.GetValueInfoByValue (value);
  }

  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    return BooleanToEnumPropertyConverter.GetValueInfoByIdentifier (identifier);
  }
}
}
