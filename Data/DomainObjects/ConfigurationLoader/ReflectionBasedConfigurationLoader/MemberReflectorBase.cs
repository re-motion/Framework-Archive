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
    private PropertyInfo _propertyInfo;

    protected MemberReflectorBase (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    protected virtual void Validate()
    {
      CheckStorageClass();
      CheckSupportedPropertyAttributes();
    }

    protected virtual void AddAttributeConstraints (Dictionary<Type, AttributeConstraint> attributeConstraints)
    {
      ArgumentUtility.CheckNotNull ("attributeConstraints", attributeConstraints);

      attributeConstraints.Add (typeof (StringAttribute), CreateAttributeConstraintForValueTypeProperty<StringAttribute, string>());
      attributeConstraints.Add (typeof (BinaryAttribute), CreateAttributeConstraintForValueTypeProperty<BinaryAttribute, byte[]>());
      attributeConstraints.Add (typeof (MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute>());
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
          _attributeConstraints = new Dictionary<Type, AttributeConstraint>();
          AddAttributeConstraints (_attributeConstraints);
        }
        return _attributeConstraints;
      }
    }

    private void CheckStorageClass()
    {
      StorageClassAttribute attribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (PropertyInfo, true);
      if (attribute != null && attribute.StorageClass != StorageClass.Persistent)
        throw CreateMappingException (null, PropertyInfo, "Only StorageClass.Persistent is supported.");
    }

    private void CheckSupportedPropertyAttributes()
    {
      foreach (Attribute attribute in AttributeUtility.GetCustomAttributes<Attribute> (PropertyInfo, true))
      {
        AttributeConstraint constraint;
        if (AttributeConstraints.TryGetValue (attribute.GetType(), out constraint))
        {
          if (!Array.Exists (constraint.PropertyTypes, delegate (Type type) { return type.IsAssignableFrom (PropertyInfo.PropertyType); }))
            throw CreateMappingException (null, PropertyInfo, constraint.Message);
        }
      }
    }

    protected bool IsRelationProperty
    {
      get { return (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType)); }
    }

    protected bool IsNullable
    {
      get
      {
        if (PropertyInfo.PropertyType.IsValueType)
          return IsNullableValueType;
        return IsNullableReferenceType;
      }
    }

    private bool IsNullableValueType
    {
      get { return typeof (INaNullable).IsAssignableFrom (PropertyInfo.PropertyType); }
    }

    private bool IsNullableReferenceType
    {
      get
      {
        INullablePropertyAttribute attribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (PropertyInfo, true);
        if (attribute != null)
          return attribute.IsNullable;
        return true;
      }
    }

    protected MappingException CreateMappingException (Exception innerException, PropertyInfo propertyInfo, string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      StringBuilder messageBuilder = new StringBuilder();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name);

      return new MappingException (messageBuilder.ToString(), innerException);
    }
  }
}