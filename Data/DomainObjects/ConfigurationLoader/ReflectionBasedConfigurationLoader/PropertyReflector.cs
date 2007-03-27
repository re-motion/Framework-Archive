using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="PropertyDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class PropertyReflector: MemberReflectorBase
  {
    public PropertyReflector (PropertyInfo propertyInfo)
        : base(propertyInfo)
    {
    }

    public PropertyDefinition GetMetadata()
    {
      Validate ();
      TypeInfo typeInfo = GetTypeInfo();

      return new PropertyDefinition (
          GetPropertyName (PropertyInfo),
          StorageSpecificName,
          typeInfo.MappingType,
          true,
          typeInfo.IsNullable,
          MaxLength,
          true);
    }

    private TypeInfo GetTypeInfo()
    {
      Type nativePropertyType = IsRelationProperty ? typeof (ObjectID) : PropertyInfo.PropertyType;
      bool isNullable = IsNullable;

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

    private string StorageSpecificName
    {
      get
      {
        StorageSpecificNameAttribute attribute = AttributeUtility.GetCustomAttribute<StorageSpecificNameAttribute> (PropertyInfo, true);
        if (attribute != null)
          return attribute.Name;
        return PropertyInfo.Name;
      }
    }

    private NaInt32 MaxLength
    {
      get
      {
        ILengthConstrainedPropertyAttribute attribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (PropertyInfo, true);
        if (attribute != null)
          return NaInt32.FromBoxedInt32 (attribute.MaximumLength);
        return NaInt32.Null;
      }
    }
  }
}