using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the properties and relations of a class.</summary>
  public abstract class MemberReflectorBase
  {
    protected sealed class AttributeConstraint
    {
      private readonly Type[] _propertyTypes;
      private readonly string _message;

      public AttributeConstraint (string message, params Type[] propertyTypes)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("message", message);
        ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("propertyTypes", propertyTypes);

        _propertyTypes = propertyTypes;
        _message = message;
      }

      public Type[] PropertyTypes
      {
        get { return _propertyTypes; }
      }

      public string Message
      {
        get { return _message; }
      }
    }

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
    // TODO: consider moving this somewhere else
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      return propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
    }

    private Dictionary<Type, AttributeConstraint> _attributeConstraints = null;

    protected MemberReflectorBase()
    {
    }

    protected virtual void Validate (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      
      CheckStorageClass (propertyInfo);
      CheckSupportedPropertyAttributes (propertyInfo);
    }

    protected virtual void AddAttributeConstraints (Dictionary<Type, AttributeConstraint> attributeConstraints)
    {
      ArgumentUtility.CheckNotNull ("attributeConstraints", attributeConstraints);

      attributeConstraints.Add (typeof (StringAttribute), CreateAttributeConstraintForValueTypeProperty<StringAttribute, string> ());
      attributeConstraints.Add (typeof (BinaryAttribute), CreateAttributeConstraintForValueTypeProperty<BinaryAttribute, byte[]> ());
      attributeConstraints.Add (typeof (MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute> ());
    }

    protected AttributeConstraint CreateAttributeConstraintForValueTypeProperty<TAttribute, TProperty>()
        where TAttribute: Attribute
    {
      return new AttributeConstraint (
          string.Format ("The {0} may be only applied to properties of type {1}.", typeof (TAttribute).FullName, typeof (TProperty).FullName),
          typeof (TProperty));
    }

    protected AttributeConstraint CreateAttributeConstraintForRelationProperty<TAttribute>()
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

    protected Dictionary<Type, AttributeConstraint> AttributeConstraints
    {
      get
      {
        if (_attributeConstraints == null)
        {
          _attributeConstraints = new Dictionary<Type, AttributeConstraint> ();
          AddAttributeConstraints (_attributeConstraints);
        }
        return _attributeConstraints;
      }
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
        if (AttributeConstraints.TryGetValue (attribute.GetType(), out constraint))
        {
          if (!Array.Exists (constraint.PropertyTypes, delegate (Type type) { return type.IsAssignableFrom (propertyInfo.PropertyType); }))
            throw CreateMappingException (null, propertyInfo, constraint.Message);
        }
      }
    }

    protected bool IsRelationProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return (typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType));
    }

    protected bool GetNullability (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      
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

    protected MappingException CreateMappingException (Exception innerException, PropertyInfo propertyInfo, string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      StringBuilder messageBuilder = new StringBuilder ();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name);

      return new MappingException (messageBuilder.ToString(), innerException);
    }
  }
}