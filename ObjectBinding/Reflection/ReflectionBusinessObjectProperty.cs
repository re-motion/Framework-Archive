using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

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

    XmlAttributeAttribute[] xmlAttributes = (XmlAttributeAttribute[]) propertyInfo.GetCustomAttributes (typeof (XmlAttributeAttribute), true);
    if (xmlAttributes.Length == 1)
    {
      XmlAttributeAttribute xmlAttribute = xmlAttributes[0];
      // create ReflectionBusinessObjectDateProperty for [XmlAttribute (DataType="date")] DateTime
      if (propertyInfo.PropertyType == typeof (DateTime) && xmlAttribute.DataType == "date")
        return new ReflectionBusinessObjectDateProperty (propertyInfo, itemType, isList, isNullableType);
    }

    if (propertyInfo.PropertyType == typeof (string) || propertyInfo.PropertyType == typeof (string[]))
      return new ReflectionBusinessObjectStringProperty (propertyInfo, itemType, isList);
    else if (itemType == typeof (int) || propertyInfo.PropertyType == typeof (int[]))
      return new ReflectionBusinessObjectInt32Property (propertyInfo, itemType, isList, isNullableType);
    else if (itemType == typeof (bool) || propertyInfo.PropertyType == typeof (bool[]))
      return new ReflectionBusinessObjectBooleanProperty (propertyInfo, itemType, isList, isNullableType);
    else if (propertyInfo.PropertyType == typeof (DateTime) || propertyInfo.PropertyType == typeof (DateTime[]))
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

public class ReflectionBusinessObjectBooleanProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectBooleanProperty
{
  public ReflectionBusinessObjectBooleanProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
    : base (propertyInfo, itemType, isList, isNullable)
  {
  }

  public bool AllowNegative
  {
    get { return false; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (! IsList && IsNullableType)
      return NaBoolean.ToBoxedBoolean ((NaBoolean)internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (! IsList && IsNullableType)
      return NaBoolean.FromBoxedBoolean (publicValue);
    else
      return publicValue;
  }
}

public class ReflectionBusinessObjectDateProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectDateProperty
{
  public ReflectionBusinessObjectDateProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
    : base (propertyInfo, itemType, isList, isNullable)
  {
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (! IsList && IsNullableType)
    {
      NaDateTime value = (NaDateTime) internalValue;
      return NaDateTime.ToBoxedDateTime (value.Date);
    }
    else
    {
      return ((DateTime)internalValue).Date;
    }
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (! IsList && IsNullableType)
      return NaDateTime.FromBoxedDateTime (publicValue).Date;
    else
      return ((DateTime)publicValue).Date;
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
  private const string c_disabledPrefix = "Disabled_";

  public ReflectionBusinessObjectEnumerationProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
    : base (propertyInfo, itemType, isList)
  {
  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return GetValues (false);
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    return GetValues (true);
  }

  private IEnumerationValueInfo[] GetValues (bool includeDisabledValues)
  {
    Debug.Assert (PropertyInfo.PropertyType.IsEnum, "type.IsEnum");
    FieldInfo[] fields = PropertyInfo.PropertyType.GetFields (BindingFlags.Static | BindingFlags.Public);
    ArrayList valueInfos = new ArrayList (fields.Length);

    foreach (FieldInfo field in fields)
    {
      bool isEnabled = ! field.Name.StartsWith (c_disabledPrefix);

      if (    ! includeDisabledValues && isEnabled
          ||  includeDisabledValues)
      {
        valueInfos.Add (
          new EnumerationValueInfo (field.GetValue (null), field.Name, field.Name, isEnabled));
      }
    }

    return (IEnumerationValueInfo[]) valueInfos.ToArray (typeof (IEnumerationValueInfo));
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="value">
  ///   An enum value that belongs to the enum identified by <see cref="ReflectionBusinessObjectProperty.PropertyType"/>.
  /// </param>
  /// <returns></returns>
  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    if (value == null)
    {
      return null;
    }
    else
    {
      string valueString = value.ToString();

      //  Test if enum value is correct type, throws an exception if not
      Enum.Parse (PropertyType, valueString, false);

      bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

      return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
    }
  }

  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    object value = Enum.Parse (PropertyType, identifier, false);

    string valueString = value.ToString();

    bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

    return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
  }

  public override bool IsRequired
  {
    get { return true; }
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
    return ReflectionBusinessObjectStorage.GetObjects (obj.GetType());
  }

  public bool SupportsSearchAvailableObjects
  {
    get
    {
      return true;
    }
  }
}

}
