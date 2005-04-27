using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

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

  public string GetDisplayName (bool value)
  {
    // default implementation that makes build work
    return value.ToString();
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaBoolean.ToBoxedBoolean ((NaBoolean)internalValue);

    return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
      return NaBoolean.FromBoxedBoolean (publicValue);

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
