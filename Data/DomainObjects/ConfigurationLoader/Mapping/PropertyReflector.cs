using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  public class PropertyReflector
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
            "The {0} may be only applied to properties of type {1}. Type: {2}, property: {3}",
            typeof (TAttribute).FullName,
            typeof (TProperty).FullName,
            propertyInfo.DeclaringType,
            propertyInfo.Name);
      }
    }

    private TypeInfo GetTypeInfo (PropertyInfo propertyInfo)
    {
      bool isNullable = GetNullabilityFromPropertyType (propertyInfo);
      if (!isNullable)
        isNullable = GetNullabilityFromAttribute (propertyInfo);

      return TypeInfo.GetMandatory (propertyInfo.PropertyType, isNullable);
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

    private bool GetNullabilityFromPropertyType(PropertyInfo propertyInfo)
    {
      return typeof (INaNullable).IsAssignableFrom (propertyInfo.PropertyType);
    }

    private bool GetNullabilityFromAttribute (PropertyInfo propertyInfo)
    {
      INullablePropertyAttribute attribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (propertyInfo, true);
      if (attribute != null)
        return attribute.IsNullable;
      return false;
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}