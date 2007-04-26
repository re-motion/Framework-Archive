using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="PropertyDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  //TODO: Validation: check that only non-virtual relation endpoints are returned as propertydefinition.
  //TODO: Test for null or empty StorageSpecificIdentifier
  public class PropertyReflector: MemberReflectorBase
  {
    public PropertyReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
    }

    public PropertyDefinition GetMetadata()
    {
      Validate();
      CheckValidPropertyType();

      return new ReflectionBasedPropertyDefinition (
          GetPropertyName(),
          GetStorageSpecificIdentifier(),
          IsRelationProperty() ? typeof (ObjectID) : PropertyInfo.PropertyType,
          IsNullable(),
          GetMaxLength(),
          true);
    }

    private void CheckValidPropertyType()
    {
      Type nativePropertyType = GetNativePropertyType();
      bool isNullable = IsNullable() ?? false;

      if (nativePropertyType.IsEnum)
        return;

      try
      {
        TypeInfo.GetMandatory (nativePropertyType, isNullable);
      }
      catch (MandatoryMappingTypeNotFoundException e)
      {
        throw CreateMappingException (e, PropertyInfo, "The property type {0} is not supported.", nativePropertyType);
      }
    }

    private Type GetNativePropertyType()
    {
      if (IsRelationProperty())
        return typeof (ObjectID);

      return TypeInfo.GetNativeType (PropertyInfo.PropertyType);
    }

    //TODO: Move adding of "ID" to RdbmsPropertyReflector
    private string GetStorageSpecificIdentifier()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.Identifier;
      if (IsRelationProperty())
        return PropertyInfo.Name + "ID";
      return PropertyInfo.Name;
    }

    protected bool? IsNullable()
    {
      if (PropertyInfo.PropertyType.IsValueType)
        return null;

      if (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
        return true;

      return IsNullableFromAttribute();
    }

    private int? GetMaxLength()
    {
      ILengthConstrainedPropertyAttribute attribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.MaximumLength;
      return null;
    }
  }
}