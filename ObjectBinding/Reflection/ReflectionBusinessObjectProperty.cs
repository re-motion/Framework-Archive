using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProperty: IBusinessObjectProperty
{
  public static ReflectionBusinessObjectProperty Create (PropertyInfo propertyInfo)
  {
    ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

    bool isList = propertyInfo.PropertyType.IsArray;
    Type elementType = isList ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
    bool isNullableType = false;
    if (NaTypeUtility.IsNaNullableType (elementType))
    {
      isNullableType = true;
      elementType = NaTypeUtility.GetBasicType (elementType);
    }

    if (propertyInfo.PropertyType == typeof (string))
      return new ReflectionBusinessObjectStringProperty (propertyInfo, elementType, isList);
    else if (elementType == typeof (int))
      return new ReflectionBusinessObjectInt32Property (propertyInfo, elementType, isList, isNullableType);
    else if (propertyInfo.PropertyType == typeof (DateTime))
      return new ReflectionBusinessObjectDateTimeProperty (propertyInfo, elementType, isList, isNullableType);
    else if (propertyInfo.PropertyType.IsEnum)
      return new ReflectionBusinessObjectEnumerationProperty (propertyInfo, elementType, isList);
    else
      return new ReflectionBusinessObjectProperty (propertyInfo, elementType, isList);
  }

  PropertyInfo _propertyInfo;
  Type _elementType;
  bool _isList;

  protected ReflectionBusinessObjectProperty (PropertyInfo propertyInfo, Type elementType, bool isList)
  {
    _propertyInfo = propertyInfo;
    _elementType = elementType;
    _isList = isList;
  }

  public bool IsList
  {
    get { return _isList; }
  }

  public IList CreateList (int count)
  {
    if (! IsList)
      throw new InvalidOperationException ("Cannot create lists for non-list properties.");
    return Array.CreateInstance (_elementType, count);
  }

  public Type ElementType
  {
    get { return _elementType; }
  }

  public virtual Type PropertyType
  {
    get { return _propertyInfo.PropertyType;  }
  }

  public string Identifier
  {
    get { return _propertyInfo.Name; }
  }

  public string DisplayName
  {
    get { return _propertyInfo.Name; }
  }

  public virtual bool IsRequired
  {
    get { return false;  }
  }

  public bool IsAccessible (IBusinessObject obj)
  {
    return true;
  }

  public bool IsReadOnly(IBusinessObject obj)
  {
    return false;
  }

  public PropertyInfo PropertyInfo
  {
    get { return _propertyInfo; }
  }

  internal protected virtual object FromInternalType (object internalValue)
  {
    return internalValue;
  }

  internal protected virtual object ToInternalType (object publicValue)
  {
    return publicValue;
  }
}

public class ReflectionBusinessObjectStringProperty: ReflectionBusinessObjectProperty, IBusinessObjectStringProperty
{
  public ReflectionBusinessObjectStringProperty (PropertyInfo propertyInfo, Type elementType, bool isList)
    : base (propertyInfo, elementType, isList)
  {
  }

  public NaInt32 MaxLength
  {
    get { return NaInt32.Null; }
  }
}

public class ReflectionBusinessObjectNullableProperty: ReflectionBusinessObjectProperty
{
  bool _isNullableType;

  public ReflectionBusinessObjectNullableProperty (PropertyInfo propertyInfo, Type elementType, bool isList, bool isNullableType)
    : base (propertyInfo, elementType, isList)
  {
    _isNullableType = isNullableType;
  }

  protected bool IsNullableType
  {
    get { return _isNullableType; }
  }

  public override bool IsRequired
  {
    get { return ! _isNullableType; }
  }
}

public class ReflectionBusinessObjectInt32Property: ReflectionBusinessObjectNullableProperty, IBusinessObjectInt32Property
{
  public ReflectionBusinessObjectInt32Property (PropertyInfo propertyInfo, Type elementType, bool isList, bool isNullable)
    : base (propertyInfo, elementType, isList, isNullable)
  {
  }

  public bool AllowNegative
  {
    get { return true; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (! IsList && IsNullableType)
      return NaInt32.ToBoxedInt32 ((NaInt32)internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (! IsList && IsNullableType)
      return NaInt32.FromBoxedInt32 (publicValue);
    else
      return publicValue;
  }
}

public class ReflectionBusinessObjectDateTimeProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectDateTimeProperty
{
  public ReflectionBusinessObjectDateTimeProperty (PropertyInfo propertyInfo, Type elementType, bool isList, bool isNullable)
    : base (propertyInfo, elementType, isList, isNullable)
  {
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (! IsList && IsNullableType)
      return NaDateTime.ToBoxedDateTime ((NaDateTime)internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (! IsList && IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue);
    else
      return publicValue;
  }
}

public class ReflectionBusinessObjectEnumerationProperty: ReflectionBusinessObjectProperty, IBusinessObjectEnumerationProperty
{
  public ReflectionBusinessObjectEnumerationProperty (PropertyInfo propertyInfo, Type elementType, bool isList)
    : base (propertyInfo, elementType, isList)
  {
  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return GetAllValues();
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    Debug.Assert (PropertyInfo.PropertyType.IsEnum, "type.IsEnum");
    FieldInfo[] fields = PropertyInfo.PropertyType.GetFields (BindingFlags.Static | BindingFlags.Public);
    IEnumerationValueInfo[] valueInfos = new IEnumerationValueInfo [fields.Length];

    for (int i = 0; i < fields.Length; ++i)
      valueInfos[i] = new EnumerationValueInfo (fields[i].GetValue (null), fields[i].Name, fields[i].Name, true);
    return valueInfos;
  }

  public IEnumerationValueInfo GetValueInfo (object value)
  {
    if (value == null)
      return null;
    else
      return new EnumerationValueInfo (value, value.ToString(), value.ToString(), true);
  }

  public IEnumerationValueInfo GetValueInfo (string identifier)
  {
    object value = Enum.Parse (PropertyType, identifier, false);
    return new EnumerationValueInfo (value, identifier, identifier, true);
  }
}

}
