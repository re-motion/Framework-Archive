using System;
using System.Reflection;
using System.Diagnostics;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProperty: IBusinessObjectProperty
{
  public static ReflectionBusinessObjectProperty Create (PropertyInfo propertyInfo)
  {
    ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
    if (propertyInfo.PropertyType == typeof (int))
      return new ReflectionBusinessObjectInt32Property (propertyInfo);
    else if (propertyInfo.PropertyType == typeof (string))
      return new ReflectionBusinessObjectStringProperty (propertyInfo);
    else if (propertyInfo.PropertyType == typeof (DateTime))
      return new ReflectionBusinessObjectDateTimeProperty (propertyInfo);
    else if (propertyInfo.PropertyType.IsEnum)
      return new ReflectionBusinessObjectEnumerationProperty (propertyInfo);
    else
      return new ReflectionBusinessObjectProperty (propertyInfo);
  }

  PropertyInfo _propertyInfo;

  protected ReflectionBusinessObjectProperty (PropertyInfo propertyInfo)
  {
    _propertyInfo = propertyInfo;
  }

  public bool IsList
  {
    get { return _propertyInfo.PropertyType.IsArray; }
  }

  public Type PropertyType
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

  public NaBoolean IsRequired
  {
    get { return NaBoolean.False;  }
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
}

public class ReflectionBusinessObjectStringProperty: ReflectionBusinessObjectProperty, IBusinessObjectStringProperty
{
  public ReflectionBusinessObjectStringProperty (PropertyInfo propertyInfo)
    : base (propertyInfo)
  {
  }

  public NaInt32 MaxLength
  {
    get { return NaInt32.Null; }
  }
}

public class ReflectionBusinessObjectDateTimeProperty: ReflectionBusinessObjectProperty, IBusinessObjectDateTimeProperty
{
  public ReflectionBusinessObjectDateTimeProperty (PropertyInfo propertyInfo)
    : base (propertyInfo)
  {
  }
}

public class ReflectionBusinessObjectInt32Property: ReflectionBusinessObjectProperty, IBusinessObjectInt32Property
{
  public ReflectionBusinessObjectInt32Property (PropertyInfo propertyInfo)
    : base (propertyInfo)
  {
  }

  public bool AllowNegative
  {
    get { return true; }
  }
}

public class ReflectionBusinessObjectEnumerationProperty: ReflectionBusinessObjectProperty, IBusinessObjectEnumerationProperty
{
  public ReflectionBusinessObjectEnumerationProperty (PropertyInfo propertyInfo)
    : base (propertyInfo)
  {
  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return GetAllValues();
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    Type type = PropertyInfo.GetType();
    Debug.Assert (type.IsEnum, "type.IsEnum");
    FieldInfo[] fields = type.GetFields (BindingFlags.Static | BindingFlags.Public);
    IEnumerationValueInfo[] valueInfos = new IEnumerationValueInfo [fields.Length];
    if (Enum.GetUnderlyingType (type) != typeof (int))
      throw new NotSupportedException ("Only Int32-based enumerations are supported by Rubicon.ObjectBinding.Reflection.");

    for (int i = 0; i < fields.Length; ++i)
      valueInfos[i] = new EnumerationValueInfo ((int)fields[i].GetValue (null), fields[i].Name, fields[i].Name, true);
    return valueInfos;
  }
}

}
