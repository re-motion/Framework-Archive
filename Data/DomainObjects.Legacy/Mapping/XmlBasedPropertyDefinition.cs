using System;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.Mapping
{
  [Serializable]
  public class XmlBasedPropertyDefinition: PropertyDefinition
  {
    [NonSerialized]
    private readonly TypeInfo _typeInfo;
    [NonSerialized]
    private readonly string _mappingTypeName;
    [NonSerialized]
    private readonly bool _isNullable;

    public XmlBasedPropertyDefinition (XmlBasedClassDefinition classDefinition, string propertyName, string columnName, string mappingTypeName)
      : this (classDefinition, propertyName, columnName, mappingTypeName, false)
    {
    }

    public XmlBasedPropertyDefinition (XmlBasedClassDefinition classDefinition, string propertyName, string columnName, string mappingTypeName, bool isNullable)
      : this (classDefinition, propertyName, columnName, mappingTypeName, true, isNullable, null)
    {
    }

    public XmlBasedPropertyDefinition (XmlBasedClassDefinition classDefinition, string propertyName, string columnName, string mappingTypeName, int? maxLength)
      : this (classDefinition, propertyName, columnName, mappingTypeName, true, false, maxLength)
    {
    }

    public XmlBasedPropertyDefinition (
        XmlBasedClassDefinition classDefinition, 
        string propertyName,
        string columnName,
        string mappingTypeName,
        bool resolveMappingType,
        bool isNullable,
        int? maxLength)
      : this (classDefinition, propertyName, columnName, mappingTypeName, resolveMappingType, isNullable, maxLength, true)
    {
    }

    public XmlBasedPropertyDefinition (
        XmlBasedClassDefinition classDefinition, string propertyName, string columnName, string mappingTypeName, bool resolveMappingType, bool isNullable, int? maxLength, bool isPersistent)
        : base (classDefinition, propertyName, columnName, maxLength, isPersistent)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mappingTypeName", mappingTypeName);

      if (resolveMappingType)
      {
        TypeInfo typeInfo = GetTypeInfo (mappingTypeName, isNullable);

        if (typeInfo.Type != typeof (string) && typeInfo.Type != typeof (byte[]) && maxLength.HasValue)
          throw CreateMappingException ("MaxLength parameter cannot be supplied with value of type '{0}'.", typeInfo.Type);

        _typeInfo = typeInfo;
      }

      _mappingTypeName = mappingTypeName;
      _isNullable = isNullable;
    }

    public override Type PropertyType
    {
      get { return _typeInfo != null ? _typeInfo.Type : null; }
    }

    public override bool IsPropertyTypeResolved
    {
      get { return (_typeInfo != null); }
    }

    public string MappingTypeName
    {
      get { return _mappingTypeName; }
    }

    public override object DefaultValue
    {
      get { return _typeInfo != null ? _typeInfo.DefaultValue : null; }
    }

    public override bool IsNullable
    {
      get { return _isNullable; }
    }

    public override bool IsObjectID
    {
      get { return _mappingTypeName == TypeInfo.ObjectIDMappingTypeName; }
    }

    private TypeInfo GetTypeInfo (string mappingTypeName, bool isNullable)
    {
      TypeInfo typeInfo = TypeInfo.GetInstance (mappingTypeName, isNullable);

      if (typeInfo != null)
        return typeInfo;

      Type type = Type.GetType (mappingTypeName, false);
      if (type != null && type.IsEnum)
      {
        TypeInfo enumTypeInfo = new TypeInfo (type, mappingTypeName, isNullable, TypeInfo.GetDefaultEnumValue (type));
        TypeInfo.AddInstance (enumTypeInfo);
        return enumTypeInfo;
      }

      string message;
      if (isNullable)
        message = string.Format ("Cannot map nullable type '{0}'.", mappingTypeName);
      else
        message = string.Format ("Cannot map not-nullable type '{0}'.", mappingTypeName);

      throw CreateMappingException (message);
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}