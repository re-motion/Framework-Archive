using System;
using Rubicon.NullableValueTypes;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class PropertyDefinition
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private string _columnName;
  private TypeInfo _typeInfo;
  private NaInt32 _maxLength;
  
  // construction and disposing

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string mappingType)
      : this (propertyName, columnName, mappingType, false)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string mappingType, 
      bool isNullable)
      : this (propertyName, columnName, mappingType, isNullable, NaInt32.Null)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string mappingType, 
      NaInt32 maxLength)
      : this (propertyName, columnName, mappingType, false, maxLength)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string mappingType, 
      bool isNullable,
      NaInt32 maxLength)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNullOrEmpty ("mappingType", mappingType);

    TypeInfo typeInfo = GetTypeInfo (mappingType, isNullable);

    if (typeInfo.Type == typeof (string) && maxLength == NaInt32.Null)
    {
      throw CreateMappingException (
          "Property '{0}' of type 'System.String' must have MaxLength defined.", propertyName);
    }

    if (typeInfo.Type != typeof (string) && typeInfo.Type != typeof (byte[]) && !maxLength.IsNull)
    {
      throw CreateMappingException (
          "MaxLength parameter cannot be supplied with value of type '{0}'.", typeInfo.Type);
    }

    _propertyName = propertyName;
    _columnName = columnName;
    _typeInfo = typeInfo;
    _maxLength = maxLength;
  }

  private TypeInfo GetTypeInfo (string mappingType, bool isNullable)
  {
    TypeInfo typeInfo = TypeInfo.GetInstance (mappingType, isNullable);

    if (typeInfo != null)
      return typeInfo;

    Type type = Type.GetType (mappingType, false);
    if (type != null && type.IsEnum)
    {
      TypeInfo enumTypeInfo = new TypeInfo (type, mappingType, isNullable, TypeInfo.GetDefaultEnumValue (type));
      TypeInfo.AddInstance (enumTypeInfo);
      return enumTypeInfo;
    }
    
    string message;
    if (isNullable)
      message = string.Format ("Cannot map nullable type '{0}'.", mappingType);
    else
      message = string.Format ("Cannot map not-nullable type '{0}'.", mappingType);

    throw CreateMappingException (message);
  }

  // methods and properties

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  private MappingException CreateMappingException (Exception innerException, string message, params object[] args)
  {
    return new MappingException (string.Format (message, args), innerException);
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
    get { return _typeInfo.Type; }
  }

  public string MappingType 
  {
    get { return _typeInfo.MappingType; }
  }

  public bool IsNullable
  {
    get { return _typeInfo.IsNullable; }
  }

  public NaInt32 MaxLength
  {
    get { return _maxLength; }
  }

  public object DefaultValue 
  {
    get { return _typeInfo.DefaultValue; }
  }
}
}
