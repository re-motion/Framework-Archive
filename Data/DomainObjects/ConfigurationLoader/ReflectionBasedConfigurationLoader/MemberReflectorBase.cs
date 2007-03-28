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

    protected string GetPropertyName ()
    {
      return ReflectionUtility.GetPropertyName (PropertyInfo);
    }

    protected bool IsRelationProperty()
    {
      return (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType));
    }

    protected bool IsNullable()
    {
      if (PropertyInfo.PropertyType.IsValueType)
        return IsNullableValueType();
      return IsNullableReferenceType();
    }

    private bool IsNullableValueType()
    {
      return typeof (INaNullable).IsAssignableFrom (PropertyInfo.PropertyType);
    }

    private bool IsNullableReferenceType()
    {
      INullablePropertyAttribute attribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.IsNullable;
      return true;
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