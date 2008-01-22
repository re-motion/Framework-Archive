using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// The implementation of <see cref="Rubicon.ObjectBinding.IBusinessObjectClassWithIdentity"/> for the <see cref="BindableDomainObject"/>.
/// </summary>
/// <remarks>
///   <para>
///     See the documentation of <see cref="Rubicon.ObjectBinding.IBusinessObjectClassWithIdentity"/> for further information.
///   </para>
///   <para>
///     This class is used with <see cref="BindableDomainObject"/> and <see cref="DomainObjectDataSource"/>.
///   </para>
/// </remarks>
public class DomainObjectClass: IBusinessObjectClassWithIdentity
{
  private static readonly List<string> s_frameworkPropertyNames;

  static DomainObjectClass ()
  {
    s_frameworkPropertyNames = new List<string> ();
    s_frameworkPropertyNames.Add ("IsDiscarded");
    s_frameworkPropertyNames.Add ("State");
    s_frameworkPropertyNames.Add ("ID");
    s_frameworkPropertyNames.Add ("DataContainer");
    s_frameworkPropertyNames.Add ("ClientTransaction");
    s_frameworkPropertyNames.Add ("IsBoundToSpecificTransaction");
  }

  public static DomainObjectClass CreateForDesignMode (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    string classID = type.Name;
    ClassIDAttribute classIDAttribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (type, false);
    if (classIDAttribute != null)
      classID = classIDAttribute.ClassID;

    return new DomainObjectClass (MappingConfiguration.Current.ClassDefinitions.GetMandatory (classID));
  }

  private BusinessObjectClassReflector _classReflector;
  private ClassDefinition _classDefinition;

  public DomainObjectClass (Type type)
    : this (MappingConfiguration.Current.ClassDefinitions.GetMandatory (ArgumentUtility.CheckNotNull ("type", type)))
  {
  }

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="classDefinition">The <see cref="ClassDefinition"/> that the object should represent.</param>
  public DomainObjectClass (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    _classDefinition = classDefinition;
    ReflectionPropertyFactory propertyFactory = null;

    // TODO: Was ist wenn der Type typeof(string) ist? 
    // BindableDomainObject hat nur protected konstruktoren und kann daher nicht instanziert werden.
    // Vorschlag:
    //    if (type.IsSubclassOf (typeof (BindableDomainObject)))
    //      propertyFactory = new DomainObjectPropertyFactory (MappingConfiguration.Current.ClassDefinitions.GetMandatory (type));
    //    else
    //      propertyFactory = new ReflectionPropertyFactory ();

    if (_classDefinition.ClassType == typeof (BindableDomainObject))
      propertyFactory = new ReflectionPropertyFactory (this);
    else
      propertyFactory = new DomainObjectPropertyFactory (this);

    _classReflector = new BusinessObjectClassReflector (_classDefinition.ClassType, propertyFactory);
  }

  /// <summary>Gets the <see cref="ClassDefinition"/> of this <see cref="DomainObjectClass"/>.</summary>
  public ClassDefinition ClassDefinition
  {
    get { return _classDefinition; }
  }

  /// <summary>
  /// Returns an <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> representing the given <paramref name="propertyIdentifier"/>.
  /// </summary>
  /// <param name="propertyIdentifier">The name of the property.</param>
  /// <returns>An instance of <see cref="BaseProperty"/> or derived type representing the given <paramref name="propertyIdentifier"/>, or <see langword="null"/> if not found.</returns>
  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    return _classReflector.GetPropertyDefinition (propertyIdentifier);
  }

  /// <summary>
  /// Returns an array of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for all properties of the type that was passed in the constructor.
  /// </summary>
  /// <returns>An array of instances of <see cref="BaseProperty"/> or a derived type for each property. If no properties can be found, an empty array is returned.</returns>
  public IBusinessObjectProperty[] GetPropertyDefinitions ()
  {
    return GetFilteredPropertyDefinitions (_classReflector.GetPropertyDefinitions ());
  }

  private IBusinessObjectProperty[] GetFilteredPropertyDefinitions (IBusinessObjectProperty[] properties)
  {
    List<IBusinessObjectProperty> resultProperties = new List<IBusinessObjectProperty> ();
    foreach (IBusinessObjectProperty property in properties)
    {
      if (!s_frameworkPropertyNames.Contains (property.Identifier))
        resultProperties.Add (property);
    }

    return resultProperties.ToArray ();
  }

  /// <summary>
  /// Returns an instance of <see cref="DomainObjectProvider"/>.
  /// </summary>
  public IBusinessObjectProvider BusinessObjectProvider 
  {
    get { return DomainObjectProvider.Instance; }
  }

  /// <summary>
  /// Returns the object with the given <paramref name="identifier"/>.
  /// </summary>
  /// <param name="identifier">The identifier of the object to return.</param>
  /// <returns>The <see cref="BindableDomainObject"/> with the given <paramref name="identifier"/>.</returns>
  /// <remarks>See <see cref="BindableDomainObject.GetObject"/> for a list of exceptions that can occur.</remarks>
  public IBusinessObjectWithIdentity GetObject (string identifier)
  {
    return BindableDomainObject.GetObject (ObjectID.Parse (identifier));
  }

  /// <summary>
  /// Gets <see langword="false"/>.
  /// </summary>
  public bool RequiresWriteBack
  { 
    get { return false; }
  }

  /// <summary>
  /// Gets the full name of the class that is represented by the object.
  /// </summary>
  /// <value>The full name of the class that is represented by the object.</value>
  public string Identifier
  {
    get { return _classReflector.Identifier; }
  }
}
}
