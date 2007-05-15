using System;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects
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
  /// Provides an encapsulation of a <see cref="DomainObject">DomainObject's</see> property for simple access.
  /// </summary>
  /// <typeparam name="T">The property value type. For simple value properties, this must be the simple property type. For related objects, this
  /// must be the related object's type. For related object collections, this must be of type <see cref="ObjectList{T}"/>, where "T" is the related
  /// objects' type.</typeparam>
  public struct PropertyAccessor<T>
  {
    private DomainObject _domainObject;
    private string _propertyIdentifier;
    private PropertyKind _kind;

    private PropertyDefinition _propertyDefinition;
    private IRelationEndPointDefinition _relationEndPointDefinition;
    private ClassDefinition _classDefinition;

    /// <summary>
    /// Initializes the <see cref="PropertyAccessor"/> object.
    /// </summary>
    /// <param name="domainObject">The domain object whose property is to be encapsulated.</param>
    /// <param name="propertyIdentifier">The identifier of the property to be encapsulated.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to the constructor is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier or the property's type
    /// does not equal the type of <typeparamref name="T"/>.</exception>
    public PropertyAccessor(DomainObject domainObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      _domainObject = domainObject;
      _propertyIdentifier = propertyIdentifier;
      _kind = PropertyKind.PropertyValue;
      _classDefinition = _domainObject.DataContainer.ClassDefinition;
      
      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects =
          PropertyAccessor.GetPropertyDefinitionObjects (_classDefinition, propertyIdentifier);

      _propertyDefinition = propertyObjects.A;
      _relationEndPointDefinition = propertyObjects.B;

      AnalyzePropertyKind ();

      Type propertyType = PropertyAccessor.GetPropertyType (_classDefinition, _propertyIdentifier);
      if (!typeof (T).Equals (propertyType))
        throw new ArgumentTypeException ("T", propertyType, typeof (T));
    }

    private void AnalyzePropertyKind ()
    {
      _kind = PropertyAccessor.GetPropertyKind (ClassDefinition, PropertyIdentifier);
    }

    /// <summary>
    /// The definition object for the property's declaring class.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get { return _domainObject.DataContainer.ClassDefinition; }
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
    /// Gets the property's value.
    /// </summary>
    /// <returns>The value of the encapsulated property.</returns>
    /// <remarks>Depending on the property's <see cref="Kind"/>, this either calls GetPropertyValue, GetRelatedObject, or GetRelatedObjects.</remarks>
    public T GetValue()
    {
      switch (Kind)
      {
        case PropertyKind.PropertyValue:
          return _domainObject.GetPropertyValue<T> (PropertyIdentifier);
        case PropertyKind.RelatedObject:
          return (T) (object) _domainObject.GetRelatedObject (PropertyIdentifier);
        default:
          Assertion.Assert (Kind == PropertyKind.RelatedObjectCollection);
          return (T) (object) _domainObject.GetRelatedObjects (PropertyIdentifier);
      }
    }

    /// <summary>
    /// Sets the property's value.
    /// </summary>
    /// <param name="value">The value to be set.</param>
    /// <remarks>Depending on the property's <see cref="Kind"/>, this either calls SetPropertyValue or SetRelatedObject.</remarks>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    public void SetValue (T value)
    {
      switch (Kind)
      {
        case PropertyKind.PropertyValue:
          _domainObject.SetPropertyValue (PropertyIdentifier, value);
          break;
        case PropertyKind.RelatedObject:
          _domainObject.SetRelatedObject (PropertyIdentifier, (DomainObject) (object) value);
          break;
        default:
          Assertion.Assert (Kind == PropertyKind.RelatedObjectCollection);
          throw new InvalidOperationException ("Related object collections cannot be set.");
      }
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
  }

  /// <summary>
  /// Provides static methods to classify properties to be accessed via <see cref="PropertyAccessor{T}"/>.
  /// </summary>
  public static class PropertyAccessor
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

      if (propertyObjects.B == null)
        return PropertyKind.PropertyValue;
      else if (propertyObjects.B.Cardinality == CardinalityType.One)
        return PropertyKind.RelatedObject;
      else
        return PropertyKind.RelatedObjectCollection;
    }

    /// <summary>
    /// Returns the value type of the property.
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

      Tuple<PropertyDefinition, IRelationEndPointDefinition> definitionObjects = GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);

      switch (GetPropertyKind(classDefinition, propertyIdentifier))
      {
        case PropertyKind.PropertyValue:
          return definitionObjects.A.PropertyType;
        default:
          return GetRelatedObjectType (definitionObjects.B);
      }
    }

    private static Type GetRelatedObjectType (IRelationEndPointDefinition endPointDefinition)
    {
      if (endPointDefinition.PropertyType.Equals (typeof (ObjectID)))
        return endPointDefinition.RelationDefinition.GetOppositeClassDefinition (endPointDefinition).ClassType;
      else
        return endPointDefinition.PropertyType;
    }

    /// <summary>
    /// Returns mapping objects for the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's <see cref="PropertyDefinition"/> and <see cref="IRelationEndPointDefinition"/> objects.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Tuple<PropertyDefinition, IRelationEndPointDefinition> GetPropertyDefinitionObjects (ClassDefinition classDefinition,
          string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);
      IRelationEndPointDefinition relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyIdentifier);

      if (propertyDefinition == null && relationEndPointDefinition == null)
      {
        string message = string.Format (
            "The domain object type {0} does not have a mapping property named {1}.",
            classDefinition.ClassType.FullName,
            propertyIdentifier);

        throw new ArgumentException (message, "propertyIdentifier");
      }
      else
      {
        return new Tuple<PropertyDefinition, IRelationEndPointDefinition> (propertyDefinition, relationEndPointDefinition);
      }
    }
  }
}
