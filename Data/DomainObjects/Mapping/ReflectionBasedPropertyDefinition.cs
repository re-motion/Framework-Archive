using System;
using System.Runtime.Serialization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedPropertyDefinition: PropertyDefinition
  {
    private Type _propertyType;
    private bool _isNullable;

    public ReflectionBasedPropertyDefinition (string propertyName, string columnName, Type propertyType)
      : this (propertyName, columnName, propertyType, null)
    {
    }

    public ReflectionBasedPropertyDefinition (string propertyName, string columnName, Type propertyType, bool isNullable)
      : this (propertyName, columnName, propertyType, isNullable, null)
    {
    }

    public ReflectionBasedPropertyDefinition (string propertyName, string columnName, Type propertyType, int? maxLength)
      : this (propertyName, columnName, propertyType, null, maxLength)
    {
    }

    public ReflectionBasedPropertyDefinition (string propertyName, string columnName, Type propertyType, bool? isNullable, int? maxLength)
      : this (propertyName, columnName, propertyType, isNullable, maxLength, true)
    {
    }

    public ReflectionBasedPropertyDefinition (string propertyName, string columnName, Type propertyType, bool? isNullable, int? maxLength, bool isPersistent)
        : base (propertyName, columnName, maxLength, isPersistent)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      if (propertyType.IsValueType && isNullable.HasValue)
        throw CreateArgumentException (propertyName, "IsNullable parameter cannot be supplied for value type properties.");
      if (propertyType != typeof (string) && propertyType != typeof (byte[]) && maxLength.HasValue)
        throw CreateArgumentException (propertyName, "MaxLength parameter cannot be supplied for properties of type '{0}'.", propertyType);

      _propertyType = propertyType;
      if (_propertyType.IsValueType)
        _isNullable = IsNullableValueType(_propertyType);
      else
        _isNullable = isNullable ?? true;
    }

    private bool IsNullableValueType (Type propertyType)
    {
      if (typeof (INaNullable).IsAssignableFrom (propertyType))
        return true;
      return propertyType.IsGenericTypeDefinition && _propertyType.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public override Type PropertyType
    {
      get { return _propertyType; }
    }

    public override string MappingTypeName
    {
      get { return _propertyType == typeof (ObjectID) ? TypeInfo.ObjectIDMappingTypeName : _propertyType.FullName; }
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
        if (_propertyType.IsEnum)
          return TypeInfo.GetDefaultEnumValue (_propertyType);
        return TypeInfo.GetMandatory (_propertyType, _isNullable).DefaultValue;
      }
    }

    public override bool IsNullable
    {
      get { return _isNullable; }
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