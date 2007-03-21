using System;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Used to create the <see cref="PropertyDefinition"/> from a single <see cref="PropertyInfo"/> or for the properties of the entire type.
  /// </summary>
  public class PropertyReflector
  {
    public PropertyReflector()
    {
    }

    public PropertyDefinition GetMetadata (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      TypeInfo typeInfo = GetTypeInfo (propertyInfo);
      Validate (propertyInfo);

      return new PropertyDefinition (
          GetPropertyName (propertyInfo),
          GetColumnName (propertyInfo),
          typeInfo.MappingType,
          true,
          typeInfo.IsNullable,
          GetMaxLength (propertyInfo));
    }

    private void Validate (PropertyInfo propertyInfo)
    {
      CheckAttributePropertyTypeCombination<string, StringPropertyAttribute> (propertyInfo);
      CheckAttributePropertyTypeCombination<byte[], BinaryPropertyAttribute> (propertyInfo);
    }

    private void CheckAttributePropertyTypeCombination<TProperty, TAttribute> (PropertyInfo propertyInfo)
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes<Attribute> (propertyInfo, true);

      if (propertyInfo.PropertyType != typeof (TProperty)
          && Array.Exists (attributes, delegate (Attribute attribute) { return attribute is TAttribute; }))
      {
        throw CreateMappingException (
            null,
            propertyInfo,
            "The {0} may be only applied to properties of type {1}.",
            typeof (TAttribute).FullName,
            typeof (TProperty).FullName);
      }
    }

    private TypeInfo GetTypeInfo (PropertyInfo propertyInfo)
    {
      bool isNullable = GetIsNullability (propertyInfo);
      Type nativePropertyType = IsRelationProperty(propertyInfo) ? typeof (ObjectID) : propertyInfo.PropertyType;

      try
      {
        return TypeInfo.GetMandatory (nativePropertyType, isNullable);
      }
      catch (MandatoryMappingTypeNotFoundException e)
      {
        throw CreateMappingException (e, propertyInfo, "The property type {0} is not supported.", nativePropertyType);
      }
    }

    private static bool IsRelationProperty(PropertyInfo propertyInfo)
    {
      return (typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType));
    }

    private bool GetIsNullability (PropertyInfo propertyInfo)
    {
      if (propertyInfo.PropertyType.IsValueType)
        return GetNullabilityForValueType (propertyInfo);
      return GetNullabilityForReferenceType (propertyInfo);
    }

    private bool GetNullabilityForValueType (PropertyInfo propertyInfo)
    {
      return typeof (INaNullable).IsAssignableFrom (propertyInfo.PropertyType);
    }

    private bool GetNullabilityForReferenceType (PropertyInfo propertyInfo)
    {
      INullablePropertyAttribute attribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (propertyInfo, true);
      if (attribute != null)
        return attribute.IsNullable;
      return true;
    }

    // TODO: consider moving this somewhere else
    /// <summary>
    /// Returns the RPF property identifier for a given property member.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned.</param>
    /// <returns>The property identifier for the given property.</returns>
    /// <remarks>Currently, the identifier is defined to be the full name of the property's declaring type, suffixed with a dot (".") and the
    /// property's name (e.g. MyNamespace.MyType.MyProperty). However, this might change in the future, so this API should be used whenever the
    /// identifier must be retrieved programmatically.</remarks>
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);    
      return propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
    }

    private string GetColumnName (PropertyInfo propertyInfo)
    {
      return propertyInfo.Name;
    }

    private NaInt32 GetMaxLength (PropertyInfo propertyInfo)
    {
      ILengthConstrainedPropertyAttribute attribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (propertyInfo, true);
      if (attribute != null)
        return NaInt32.FromBoxedInt32 (attribute.MaximumLength);
      return NaInt32.Null;
    }

    private MappingException CreateMappingException (Exception innerException, PropertyInfo propertyInfo, string message, params object[] args)
    {
      StringBuilder messageBuilder = new StringBuilder();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name);

      return new MappingException (messageBuilder.ToString(), innerException);
    }
  }
}