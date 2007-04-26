using System;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedPropertyDefinition: PropertyDefinition
  {
    private Type _propertyType;
    private bool _isNullable;

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType)
        : this (classDefinition, propertyName, columnName, propertyType, null, null, true)
    {
    }

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable)
        : this (classDefinition, propertyName, columnName, propertyType, isNullable, null, true)
    {
    }

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, int maxLength)
        : this (classDefinition, propertyName, columnName, propertyType, null, maxLength, true)
    {
    }

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable, int maxLength)
        : this (classDefinition, propertyName, columnName, propertyType, isNullable, maxLength, true)
    {
    }

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        bool? isNullable,
        int? maxLength,
        bool isPersistent)
        : base (classDefinition, propertyName, columnName, maxLength, isPersistent)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      if (propertyType.IsValueType && isNullable.HasValue)
      {
        throw CreateArgumentException (
            propertyName, "IsNullable parameter can only be supplied for reference types but the property is of type '{0}'.", propertyType);
      }
      //TODO: change byte[] to all arrays. Will have repurcussions in several places -> Search for byte[]
      if (propertyType != typeof (string) && propertyType != typeof (byte[]) && maxLength.HasValue)
      {
        throw CreateArgumentException (
            propertyName, "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type '{0}'.", propertyType);
      }

      _propertyType = propertyType;
      if (propertyType.IsValueType)
        _isNullable = TypeInfo.IsNullableValueType (propertyType);
      else
        _isNullable = isNullable ?? true;
    }

    public override Type PropertyType
    {
      get { return _propertyType; }
    }

    public override bool IsPropertyTypeResolved
    {
      get { return true; }
    }

    //TODO: Implement DefaultValue.
    public override object DefaultValue
    {
      get
      {
        if (_isNullable)
          return null;

        Type nativeType = TypeInfo.GetNativeType (_propertyType);
        if (nativeType.IsEnum)
          return TypeInfo.GetDefaultEnumValue (nativeType);
        return TypeInfo.GetMandatory (nativeType, false).DefaultValue;
      }
    }

    public override bool IsNullable
    {
      get { return _isNullable; }
    }

    public override bool IsObjectID
    {
      get { return _propertyType == typeof (ObjectID); }
    }

    private ArgumentException CreateArgumentException (string propertyName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args) + "\r\n  Property: " + propertyName);
    }

    #region ISerializable Members

    protected ReflectionBasedPropertyDefinition (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      if (!IsPartOfMappingConfiguration)
      {
        _propertyType = (Type) info.GetValue ("PropertyType", typeof (Type));
        _isNullable = info.GetBoolean ("IsNullable");
      }
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);

      info.AddValue ("PropertyType", _propertyType);
      info.AddValue ("IsNullable", _isNullable);
    }

    #endregion
  }
}