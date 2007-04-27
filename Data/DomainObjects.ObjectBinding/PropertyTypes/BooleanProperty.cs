using System;
using System.Reflection;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
[MultiLingualResources ("Rubicon.Data.DomainObjects.ObjectBinding.Globalization.BooleanProperty")]
public class BooleanProperty : NullableProperty, IBusinessObjectBooleanProperty, IBusinessObjectEnumerationProperty
{
  private BooleanToEnumPropertyConverter _booleanToEnumConverter;

  public BooleanProperty (      
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, isRequired, itemType, isList, isNullableType)
  {
    _booleanToEnumConverter = new BooleanToEnumPropertyConverter (this);
  }

  public string GetDisplayName (bool value)
  {
    string resourceName = value ? "True" : "False";
    return MultiLingualResourcesAttribute.GetResourceText (this, resourceName);
  }

  public bool? GetDefaultValue (IBusinessObjectClass objectClass)
  {
    if (IsNullableType)
      return null;

    return false;
  }

  public override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNaNullableType)
      return NaBoolean.ToBoxedBoolean ((NaBoolean)internalValue);

    return internalValue;
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNaNullableType)
      return NaBoolean.FromBoxedBoolean (publicValue);

    return publicValue;
  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return _booleanToEnumConverter.GetValues ();
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    return _booleanToEnumConverter.GetValues ();
  }

  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    return _booleanToEnumConverter.GetValueInfoByValue (value);
  }

  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    return _booleanToEnumConverter.GetValueInfoByIdentifier (identifier);
  }
}
}
