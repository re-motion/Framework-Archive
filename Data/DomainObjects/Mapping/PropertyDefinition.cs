using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
{
public class PropertyDefinition
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private string _columnName;
  private Type _propertyType;
  private bool _isNullable;
  private NaInt32 _maxLength;

  // construction and disposing

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      Type propertyType)
      : this (propertyName, columnName, propertyType, false)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      Type propertyType, 
      bool isNullable)
      : this (propertyName, columnName, propertyType, isNullable, NaInt32.Null)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      Type propertyType, 
      NaInt32 maxLength)
      : this (propertyName, columnName, propertyType, false, maxLength)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      Type propertyType, 
      bool isNullable,
      NaInt32 maxLength)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNull ("propertyType", propertyType);
    
    if (propertyType == typeof (string) && maxLength == NaInt32.Null)
    {
      throw CreateMappingException (
          "Property '{0}' of type 'System.String' must have MaxLength defined.", propertyName);
    }

    if (propertyType != typeof (string) && !maxLength.IsNull)
    {
      throw CreateMappingException (
          "MaxLength parameter cannot be supplied with value of type '{0}'.", propertyType);
    }

    _propertyName = propertyName;
    _columnName = columnName;
    _propertyType = GetPropertyType (propertyType, isNullable);
    _isNullable = isNullable;
    _maxLength = maxLength;
  }

  // methods and properties

  private Type GetPropertyType (Type type, bool isNullable)
  {
    if (isNullable)
    {
      if (type == typeof (bool))
        return typeof (NaBoolean);

      if (type == typeof (int))
        return typeof (NaInt32);

      if (type == typeof (double))
        return typeof (NaDouble);

      if (type == typeof (DateTime))
        return typeof (NaDateTime);

      if (type == typeof (string))
        return type;

      throw CreateNotImplementedException ("IsNullable cannot be set to true for type '{0}'.", type);
    }

    return type;
  }

  private NotImplementedException CreateNotImplementedException (string message, params object[] args)
  {
    return new NotImplementedException (string.Format (message, args));
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public string ColumnName
  {
    get { return _columnName; }
  }

  public Type PropertyType
  {
    get { return _propertyType; }
  }

  public bool IsNullable
  {
    get { return _isNullable; }
  }

  public NaInt32 MaxLength
  {
    get { return _maxLength; }
  }
}
}
