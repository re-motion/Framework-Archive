using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="PropertyDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class PropertyReflector: MemberReflectorBase
  {
    public PropertyReflector()
    {
    }

    public PropertyDefinition GetMetadata (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Validate (propertyInfo);
      TypeInfo typeInfo = GetTypeInfo (propertyInfo);

      return new PropertyDefinition (
          GetPropertyName (propertyInfo),
          GetColumnName (propertyInfo),
          typeInfo.MappingType,
          true,
          typeInfo.IsNullable,
          GetMaxLength (propertyInfo),
          true);
    }

    private TypeInfo GetTypeInfo (PropertyInfo propertyInfo)
    {
      Type nativePropertyType = IsRelationProperty (propertyInfo) ? typeof (ObjectID) : propertyInfo.PropertyType;
      bool isNullable = GetNullability (propertyInfo);

      if (nativePropertyType.IsEnum)
        return GetEnumTypeInfo (nativePropertyType, isNullable);

      try
      {
        return TypeInfo.GetMandatory (nativePropertyType, isNullable);
      }
      catch (MandatoryMappingTypeNotFoundException e)
      {
        throw CreateMappingException (e, propertyInfo, "The property type {0} is not supported.", nativePropertyType);
      }
    }

    private TypeInfo GetEnumTypeInfo (Type type, bool isNullable)
    {
      return new TypeInfo (type, TypeUtility.GetPartialAssemblyQualifiedName (type), isNullable, TypeInfo.GetDefaultEnumValue (type));
    }

    private string GetColumnName (PropertyInfo propertyInfo)
    {
      StorageSpecificNameAttribute attribute = AttributeUtility.GetCustomAttribute<StorageSpecificNameAttribute> (propertyInfo, true);
      if (attribute != null)
        return attribute.Name;
      return propertyInfo.Name;
    }

    private NaInt32 GetMaxLength (PropertyInfo propertyInfo)
    {
      ILengthConstrainedPropertyAttribute attribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (propertyInfo, true);
      if (attribute != null)
        return NaInt32.FromBoxedInt32 (attribute.MaximumLength);
      return NaInt32.Null;
    }
  }
}