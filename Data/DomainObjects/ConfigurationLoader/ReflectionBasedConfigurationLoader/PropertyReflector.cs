using System;
using System.Collections.Generic;
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
    private sealed class AttributeConstraint
    {
      public readonly Type[] PropertyTypes;
      public readonly string Message;

      public AttributeConstraint (string message, params Type[] propertyTypes)
      {
        PropertyTypes = propertyTypes;
        Message = message;
      }
    }

    private static Dictionary<Type, AttributeConstraint> s_attributeConstraints = new Dictionary<Type, AttributeConstraint>();

    static PropertyReflector()
    {
      s_attributeConstraints.Add (typeof (StringAttribute), CreateAttributeConstraintForValueTypeProperty<StringAttribute, string>());
      s_attributeConstraints.Add (typeof (BinaryAttribute), CreateAttributeConstraintForValueTypeProperty<BinaryAttribute, byte[]>());
      s_attributeConstraints.Add (typeof (MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute>());
    }

    // TODO: consider moving this somewhere else
    /// <summary>
    /// Returns the RPF property identifier for a given property member.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned.</param>
    /// <returns>The property identifier for the given property.</returns>
    /// <remarks>
    /// Currently, the identifier is defined to be the full name of the property's declaring type, suffixed with a dot (".") and the
    /// property's name (e.g. MyNamespace.MyType.MyProperty). However, this might change in the future, so this API should be used whenever the
    /// identifier must be retrieved programmatically.
    /// </remarks>
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      return propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
    }

    private static AttributeConstraint CreateAttributeConstraintForValueTypeProperty<TAttribute, TProperty>()
        where TAttribute: Attribute
    {
      return new AttributeConstraint (
          string.Format ("The {0} may be only applied to properties of type {1}.", typeof (TAttribute).FullName, typeof (TProperty).FullName),
          typeof (TProperty));
    }

    private static AttributeConstraint CreateAttributeConstraintForRelationProperty<TAttribute>()
        where TAttribute: Attribute
    {
      return new AttributeConstraint (
          string.Format (
              "The {0} may be only applied to properties assignable to types {1} or {2}.",
              typeof (TAttribute).FullName,
              typeof (DomainObject).FullName,
              typeof (DomainObjectCollection).FullName),
          typeof (DomainObject),
          typeof (DomainObjectCollection));
    }

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

    private void Validate (PropertyInfo propertyInfo)
    {
      CheckStorageClass (propertyInfo);
      CheckSupportedPropertyAttributes (propertyInfo);
    }

    private void CheckStorageClass (PropertyInfo propertyInfo)
    {
      StorageClassAttribute attribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);
      if (attribute != null && attribute.StorageClass != StorageClass.Persistent)
        throw CreateMappingException (null, propertyInfo, "Only StorageClass.Persistent is supported.");
    }

    private void CheckSupportedPropertyAttributes (PropertyInfo propertyInfo)
    {
      foreach (Attribute attribute in AttributeUtility.GetCustomAttributes<Attribute> (propertyInfo, true))
      {
        AttributeConstraint constraint;
        if (s_attributeConstraints.TryGetValue (attribute.GetType(), out constraint))
        {
          if (!Array.Exists (constraint.PropertyTypes, delegate (Type type) { return type.IsAssignableFrom (propertyInfo.PropertyType); }))
            throw CreateMappingException (null, propertyInfo, constraint.Message);
        }
      }
    }

    private TypeInfo GetTypeInfo (PropertyInfo propertyInfo)
    {
      Type nativePropertyType = IsRelationProperty (propertyInfo) ? typeof (ObjectID) : propertyInfo.PropertyType;
      bool isNullable = GetIsNullability (propertyInfo);

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

    private bool IsRelationProperty (PropertyInfo propertyInfo)
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