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
  private Type _propertyType;
  private string _mappingType;
  private bool _isNullable;
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

    TypeInfo typeInfo = new TypeInfo (mappingType, isNullable);
    
    if (typeInfo.Type == typeof (string) && maxLength == NaInt32.Null)
    {
      throw CreateMappingException (
          "Property '{0}' of type 'System.String' must have MaxLength defined.", propertyName);
    }

    if (typeInfo.Type != typeof (string) && !maxLength.IsNull)
    {
      throw CreateMappingException (
          "MaxLength parameter cannot be supplied with value of type '{0}'.", typeInfo.Type);
    }

    _propertyName = propertyName;
    _columnName = columnName;
    _propertyType = typeInfo.Type;
    _mappingType = mappingType;
    _isNullable = isNullable;
    _maxLength = maxLength;
  }

  // methods and properties

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

  public string MappingType 
  {
    get { return _mappingType; }
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
