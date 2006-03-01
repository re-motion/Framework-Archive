using System;
using System.Runtime.Serialization;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class PropertyDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;
  private string _propertyName;
  private string _columnName;
  private TypeInfo _typeInfo;
  private string _propertyTypeName;
  private bool _isNullable;
  private NaInt32 _maxLength;
  
  // Note: _mappingClassID is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private string _mappingClassID;
  
  // construction and disposing

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string propertyTypeName)
      : this (propertyName, columnName, propertyTypeName, false)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string propertyTypeName, 
      bool isNullable)
      : this (propertyName, columnName, propertyTypeName, isNullable, NaInt32.Null, true)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string propertyTypeName, 
      NaInt32 maxLength)
      : this (propertyName, columnName, propertyTypeName, false, maxLength, true)
  {
  }

  public PropertyDefinition (
      string propertyName, 
      string columnName, 
      string propertyTypeName, 
      bool isNullable,
      NaInt32 maxLength,
      bool resolvePropertyTypeName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyTypeName", propertyTypeName);

    if (resolvePropertyTypeName)
    {
      TypeInfo typeInfo = GetTypeInfo (propertyTypeName, isNullable);

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

      _typeInfo = typeInfo;
    }

    _propertyName = propertyName;
    _columnName = columnName;
    _propertyTypeName = propertyTypeName;
    _isNullable = isNullable;
    _maxLength = maxLength;
  }

  protected PropertyDefinition (SerializationInfo info, StreamingContext context)
  {
    _propertyName = info.GetString ("PropertyName");
    bool ispartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (ispartOfMappingConfiguration)
    {
      // Note: If this object was part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is deserialized here.
      _mappingClassID = info.GetString ("MappingClassID");
    }
    else
    {
      _classDefinition = (ClassDefinition) info.GetValue ("ClassDefinition", typeof (ClassDefinition));
      _columnName = info.GetString ("ColumnName");

      // GetTypeInfo must be used, to ensure enums are registered even object is deserialized into another process.
      _propertyTypeName = info.GetString ("PropertyTypeName");
      _isNullable = info.GetBoolean ("IsNullable");

      if (info.GetBoolean ("HasTypeInfo"))
        _typeInfo = GetTypeInfo (_propertyTypeName, _isNullable);
      
      _maxLength = (NaInt32) info.GetValue ("MaxLength", typeof (NaInt32));
    }
  }

  private TypeInfo GetTypeInfo (string propertyTypeName, bool isNullable)
  {
    TypeInfo typeInfo = TypeInfo.GetInstance (propertyTypeName, isNullable);

    if (typeInfo != null)
      return typeInfo;

    Type type = Type.GetType (propertyTypeName, false);
    if (type != null && type.IsEnum)
    {
      TypeInfo enumTypeInfo = new TypeInfo (type, propertyTypeName, isNullable, TypeInfo.GetDefaultEnumValue (type));
      TypeInfo.AddInstance (enumTypeInfo);
      return enumTypeInfo;
    }
    
    string message;
    if (isNullable)
      message = string.Format ("Cannot map nullable type '{0}'.", propertyTypeName);
    else
      message = string.Format ("Cannot map not-nullable type '{0}'.", propertyTypeName);

    throw CreateMappingException (message);
  }

  // methods and properties

  public ClassDefinition ClassDefinition 
  {
    get { return _classDefinition; }
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
    get { return _typeInfo != null ? _typeInfo.Type : null; }
  }

  public bool IsPropertyTypeResolved
  {
    get { return (_typeInfo != null); }
  }

  public string PropertyTypeName 
  {
    get { return _propertyTypeName; }
  }

  public bool IsNullable
  {
    get { return _isNullable; }
  }

  public NaInt32 MaxLength
  {
    get { return _maxLength; }
  }

  public object DefaultValue 
  {
    get { return _typeInfo != null ? _typeInfo.DefaultValue : null; }
  }

  public void SetClassDefinition (ClassDefinition classDefinition)
  {
    _classDefinition = classDefinition;
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  private MappingException CreateMappingException (Exception innerException, string message, params object[] args)
  {
    return new MappingException (string.Format (message, args), innerException);
  }

  #region ISerializable Members

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    GetObjectData (info, context);
  }

  protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("PropertyName", _propertyName);

    bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

    if (isPartOfMappingConfiguration)
    {
      // Note: If this object is part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is serialized here.
      info.AddValue ("MappingClassID", _classDefinition.ID);
    }
    else
    {
      info.AddValue ("ClassDefinition", _classDefinition);
      info.AddValue ("ColumnName", _columnName);
      info.AddValue ("HasTypeInfo", _typeInfo != null);
      info.AddValue ("PropertyTypeName", _propertyTypeName);
      info.AddValue ("IsNullable", _isNullable);
      info.AddValue ("MaxLength", _maxLength);
    }
  }

  #endregion

  #region IObjectReference Members

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    // Note: A PropertyDefinition must know its ClassDefinition to correctly deserialize itself and a 
    // ClassDefinition knows its PropertyDefintions. For bi-directional relationships
    // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
    // Therefore the member _classDefinition cannot be used here, because it could point to the wrong instance. 
    if (_mappingClassID != null)
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_mappingClassID)[_propertyName];
    else
      return this;
  }

  #endregion
}
}
