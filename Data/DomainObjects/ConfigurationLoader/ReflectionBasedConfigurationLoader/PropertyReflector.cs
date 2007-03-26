using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Used to create the <see cref="PropertyDefinition"/> from a single <see cref="PropertyInfo"/> or for the properties of the entire type.
  /// </summary>
  public class PropertyReflector: BaseReflector
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