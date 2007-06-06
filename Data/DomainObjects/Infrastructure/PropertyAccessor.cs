using System;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Indicates the kind of a <see cref="DomainObject">DomainObject's</see> property.
  /// </summary>
  public enum PropertyKind
  {
    /// <summary>
    /// The property is a simple value.
    /// </summary>
    PropertyValue,
    /// <summary>
    /// The property is a single related domain object.
    /// </summary>
    RelatedObject,
    /// <summary>
    /// The property is a collection of related domain objects.
    /// </summary>
    RelatedObjectCollection
  }

  /// <summary>
  /// Provides an encapsulation of a <see cref="DomainObject">DomainObject's</see> property for simple access as well as static methods
  /// supporting working with properties.
  /// </summary>
  public struct PropertyAccessor
  {
    /// <summary>
    /// Gets the <see cref="PropertyKind"/> for a given property identifier and class definition.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The <see cref="PropertyKind"/> of the property.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public static PropertyKind GetPropertyKind (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects = GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      return GetPropertyKind (propertyObjects.B);
    }

    private static PropertyKind GetPropertyKind (IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition == null)
        return PropertyKind.PropertyValue;
      else if (relationEndPointDefinition.Cardinality == CardinalityType.One)
        return PropertyKind.RelatedObject;
      else
        return PropertyKind.RelatedObjectCollection;
    }

    private static IPropertyAccessorStrategy GetStrategy (PropertyKind kind)
    {
      switch (kind)
      {
        case PropertyKind.PropertyValue:
          return ValuePropertyAccessorStrategy.Instance;
        case PropertyKind.RelatedObject:
          return RelatedObjectPropertyAccessorStrategy.Instance;
        default:
          Assertion.Assert (kind == PropertyKind.RelatedObjectCollection);
          return RelatedObjectCollectionPropertyAccessorStrategy.Instance;
      }
    }

    /// <summary>
    /// Returns the value type of the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's value type.</returns>
    /// <remarks>For simple value properties, this returns the simple property type. For related objects, it
    /// returns the related object's type. For related object collections, it returns type <see cref="ObjectList{T}"/>, where "T" is the related
    /// objects' type.</remarks>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Type GetPropertyType (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> definitionObjects =
        PropertyAccessor.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);

      return GetStrategy (GetPropertyKind (definitionObjects.B)).GetPropertyType (definitionObjects.A, definitionObjects.B);
    }

    /// <summary>
    /// Returns mapping objects for the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's <see cref="PropertyDefinition"/> and <see cref="IRelationEndPointDefinition"/> objects.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Tuple<PropertyDefinition, IRelationEndPointDefinition> GetPropertyDefinitionObjects (
        ClassDefinition classDefinition,
        string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);
      IRelationEndPointDefinition relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyIdentifier);

      if (propertyDefinition == null && relationEndPointDefinition == null)
      {
        string message = string.Format (
            "The domain object type {0} does not have a mapping property named '{1}'.",
            classDefinition.ClassType.FullName,
            propertyIdentifier);

        throw new ArgumentException (message, "propertyIdentifier");
      }
      else
        return new Tuple<PropertyDefinition, IRelationEndPointDefinition> (propertyDefinition, relationEndPointDefinition);
    }

    /// <summary>
    /// Checks whether the given property identifier denotes an existing property on the given <see cref="ClassDefinition"/>.
    /// </summary>
    /// <param name="classDefinition">The class definition to be checked.</param>
    /// <param name="propertyID">The property to be looked for.</param>
    /// <returns>True if <paramref name="classDefinition"/> contains a simple, related object, or related object collection property
    /// with the given identifier; false otherwise.</returns>
    public static bool IsValidProperty (ClassDefinition classDefinition, string propertyID)
    {
      return classDefinition.GetPropertyDefinition (propertyID) != null || classDefinition.GetRelationEndPointDefinition (propertyID) != null;
    }

    private DomainObject _domainObject;
    private string _propertyIdentifier;
    private PropertyKind _kind;

    private PropertyDefinition _propertyDefinition;
    private IRelationEndPointDefinition _relationEndPointDefinition;
    private ClassDefinition _classDefinition;
    private Type _propertyType;

    private IPropertyAccessorStrategy _strategy;

    /// <summary>
    /// Initializes the <see cref="PropertyAccessor"/> object.
    /// </summary>
    /// <param name="domainObject">The domain object whose property is to be encapsulated.</param>
    /// <param name="propertyIdentifier">The identifier of the property to be encapsulated.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to the constructor is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    internal PropertyAccessor (DomainObject domainObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      _domainObject = domainObject;
      _propertyIdentifier = propertyIdentifier;
      _classDefinition = _domainObject.DataContainer.ClassDefinition;
      _kind = PropertyAccessor.GetPropertyKind (_classDefinition, _propertyIdentifier);

      _strategy = PropertyAccessor.GetStrategy (_kind);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects =
          PropertyAccessor.GetPropertyDefinitionObjects (_classDefinition, propertyIdentifier);

      _propertyDefinition = propertyObjects.A;
      _relationEndPointDefinition = propertyObjects.B;

      _propertyType = _strategy.GetPropertyType (_propertyDefinition, _relationEndPointDefinition);
    }

    /// <summary>
    /// The definition object for the property's declaring class.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    /// <summary>
    /// Indicates which kind of property is encapsulated by this structure.
    /// </summary>
    public PropertyKind Kind
    {
      get { return _kind; }
    }

    /// <summary>
    /// The identifier for the property encapsulated by this structure.
    /// </summary>
    public string PropertyIdentifier
    {
      get { return _propertyIdentifier; }
    }

    /// <summary>
    /// The property value type. For simple value properties, this is the simple property type. For related objects, this
    /// is the related object's type. For related object collections, this is <see cref="ObjectList{T}"/>, where "T" is the
    /// related objects' type.
    /// </summary>
    public Type PropertyType
    {
      get { return _propertyType; }
    }

    /// <summary>
    /// The encapsulated object's property definition object (can be <see langword="null"/>).
    /// </summary>
    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    /// <summary>
    /// The encapsulated object's relation end point definition object (can be <see langword="null"/>).
    /// </summary>
    public IRelationEndPointDefinition RelationEndPointDefinition
    {
      get { return _relationEndPointDefinition; }
    }

    /// <summary>
    /// Gets the domain object of this property.
    /// </summary>
    /// <value>The domain object this <see cref="PropertyAccessor"/> is associated with.</value>
    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    /// <summary>
    /// Indicates whether the property's value has been changed in its current transaction.
    /// </summary>
    /// <value>True if the property's value has changed; false otherwise.</value>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public bool HasChanged
    {
      get
      {
        DomainObject.CheckIfObjectIsDiscarded();
        return _strategy.HasChanged (this);
      }
    }

    /// <summary>
    /// Gets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <returns>The value of the encapsulated property.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetValue<T> ()
    {
      if (!PropertyType.Equals (typeof (T)))
        throw new InvalidTypeException (PropertyIdentifier, typeof (T), PropertyType);

      object value = GetValueWithoutTypeCheck();
      Assertion.DebugAssert (
          !(value == null && PropertyType.IsValueType),
          "Property '{0}' is a value type but the DataContainer returned null.",
          PropertyIdentifier);
      Assertion.DebugAssert (value == null || value is T);
      return (T) value;
    }

    /// <summary>
    /// Sets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="value">The value to be set.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public void SetValue<T> (T value)
    {
      if (!PropertyType.Equals (typeof (T)))
        throw new InvalidTypeException (PropertyIdentifier, typeof (T), PropertyType);

      SetValueWithoutTypeCheck (value);
    }

    internal object GetValueWithoutTypeCheck ()
    {
      _domainObject.CheckIfObjectIsDiscarded();
      return _strategy.GetValueWithoutTypeCheck (this);
    }

    internal void SetValueWithoutTypeCheck (object value)
    {
      _domainObject.CheckIfObjectIsDiscarded();
      _strategy.SetValueWithoutTypeCheck (this, value);
    }

    /// <summary>
    /// Gets the property's value from that moment when the property's domain object was enlisted in its current <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetOriginalValue<T> ()
    {
      if (!PropertyType.Equals (typeof (T)))
        throw new InvalidTypeException (PropertyIdentifier, typeof (T), PropertyType);

      return (T) GetOriginalValueWithoutTypeCheck();
    }

    internal object GetOriginalValueWithoutTypeCheck ()
    {
      _domainObject.CheckIfObjectIsDiscarded();
      return _strategy.GetOriginalValueWithoutTypeCheck (this);
    }
  }
}
