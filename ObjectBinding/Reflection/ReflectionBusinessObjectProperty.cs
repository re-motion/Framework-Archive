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
    Type itemType = isList ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
    bool isNullableType = false;
    if (NaTypeUtility.IsNaNullableType (itemType))
    {
      isNullableType = true;
      itemType = NaTypeUtility.GetBasicType (itemType);
    }

    if (propertyInfo.PropertyType == typeof (string))
      return new ReflectionBusinessObjectStringProperty (propertyInfo, itemType, isList);
    else if (itemType == typeof (int))
      return new ReflectionBusinessObjectInt32Property (propertyInfo, itemType, isList, isNullableType);
    else if (propertyInfo.PropertyType == typeof (DateTime))
      return new ReflectionBusinessObjectDateTimeProperty (propertyInfo, itemType, isList, isNullableType);
    else if (propertyInfo.PropertyType.IsEnum)
      return new ReflectionBusinessObjectEnumerationProperty (propertyInfo, itemType, isList);
    else if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (propertyInfo.PropertyType))
      return new ReflectionBusinessObjectRefernceProperty (propertyInfo, itemType, isList);
    else
      return new ReflectionBusinessObjectProperty (propertyInfo, itemType, isList);
  }

  PropertyInfo _propertyInfo;
  Type _itemType;
  bool _isList;

  protected ReflectionBusinessObjectProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
  {
    _propertyInfo = propertyInfo;
    _itemType = itemType;
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
    return Array.CreateInstance (_itemType, count);
  }

  public Type ItemType
  {
    get { return _itemType; }
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
  public ReflectionBusinessObjectStringProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
    : base (propertyInfo, itemType, isList)
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

  public ReflectionBusinessObjectNullableProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullableType)
    : base (propertyInfo, itemType, isList)
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
  public ReflectionBusinessObjectInt32Property (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
    : base (propertyInfo, itemType, isList, isNullable)
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
  public ReflectionBusinessObjectDateTimeProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
    : base (propertyInfo, itemType, isList, isNullable)
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
  public ReflectionBusinessObjectEnumerationProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
    : base (propertyInfo, itemType, isList)
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

  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    if (value == null)
      return null;
    else
      return new EnumerationValueInfo (value, value.ToString(), value.ToString(), true);
  }

  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    object value = Enum.Parse (PropertyType, identifier, false);
    return new EnumerationValueInfo (value, identifier, identifier, true);
  }
}

public class ReflectionBusinessObjectRefernceProperty: ReflectionBusinessObjectProperty, IBusinessObjectReferenceProperty
{
  public ReflectionBusinessObjectRefernceProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
    : base (propertyInfo, itemType, isList)
  {
  }

  public IBusinessObjectClass ReferenceClass
  {
    get
    {
      return new ReflectionBusinessObjectClass (this.PropertyType);
    }
  }

  public IBusinessObjectWithIdentity[] SearchAvailableObjects(IBusinessObject obj, string searchStatement)
  {
    // TODO:  Add ReflectionBusinessObjectRefernceProperty.SearchAvailableObjects implementation
    return null;
  }

  public bool SupportsSearchAvailableObjects
  {
    get
    {
      // TODO:  Add ReflectionBusinessObjectRefernceProperty.SupportsSearchAvailableObjects getter implementation
      return false;
    }
  }
}

}
