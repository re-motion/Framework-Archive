using System;
using System.Reflection;
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

}
