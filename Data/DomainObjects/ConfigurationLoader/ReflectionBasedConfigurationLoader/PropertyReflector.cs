using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
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
      TypeInfo typeInfo = GetTypeInfo();

      return new ReflectionBasedPropertyDefinition (
          GetPropertyName(),
          GetStorageSpecificIdentifier(),
          typeInfo.Type,
          IsNullable (typeInfo),
          GetMaxLength(),
          true);
    }

    private TypeInfo GetTypeInfo()
    {
      Type nativePropertyType = IsRelationProperty() ? typeof (ObjectID) : PropertyInfo.PropertyType;
      bool isNullable = IsNullable();

      if (nativePropertyType.IsEnum)
        return GetEnumTypeInfo (nativePropertyType, isNullable);

      try
      {
        return TypeInfo.GetMandatory (nativePropertyType, isNullable);
      }
      catch (MandatoryMappingTypeNotFoundException e)
      {
        throw CreateMappingException (e, PropertyInfo, "The property type {0} is not supported.", nativePropertyType);
      }
    }

    private TypeInfo GetEnumTypeInfo (Type type, bool isNullable)
    {
      return new TypeInfo (type, TypeUtility.GetPartialAssemblyQualifiedName (type), isNullable, TypeInfo.GetDefaultEnumValue (type));
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

    private bool? IsNullable (TypeInfo typeInfo)
    {
      if (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
        return true;
      if (typeInfo.Type.IsClass)
        return typeInfo.IsNullable;
      return null;
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