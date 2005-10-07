using System;
using System.ComponentModel;

using Rubicon.ObjectBinding;
using Rubicon.Utilities;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A <see cref="DomainObject"/> that supports 2-way data binding of user controls.
/// </summary>
public class BindableDomainObject: DomainObject, IBusinessObjectWithIdentity
{
  // types

  // static members and constants

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that should be loaded.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <i>id</i> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  internal protected static new BindableDomainObject GetObject (ObjectID id)
  {
    return (BindableDomainObject) DomainObject.GetObject (id);
  }

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that should be loaded.</param>
  /// <param name="includeDeleted">Indicates if the method should return <b>BindableDomainObject</b>s that are already deleted.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <i>id</i> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  internal protected static new BindableDomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return (BindableDomainObject) DomainObject.GetObject (id, includeDeleted);
  }

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that is loaded.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <b>BindableDomainObject</b>.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> or <i>clientTransaction</i>is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <i>id</i> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  internal protected static new BindableDomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (BindableDomainObject) DomainObject.GetObject (id, clientTransaction);
  }

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that is loaded.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that us used to load the <b>BindableDomainObject</b>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <b>BindableDomainObject</b>s that are already deleted.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> or <i>clientTransaction</i>is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <i>id</i> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  internal protected static new BindableDomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return (BindableDomainObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  private BusinessObjectReflector _objectReflector;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>BindableDomainObject</b>.
  /// </summary>
  protected BindableDomainObject ()
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  /// <summary>
  /// Initializes a new <b>BindableDomainObject</b>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <b>BindableDomainObject</b> should be part of.</param>
  /// <exception cref="System.ArgumentNullException"><i>clientTransaction</i> is a null reference.</exception>
  protected BindableDomainObject (ClientTransaction clientTransaction) : base (clientTransaction)
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  /// <summary>
  /// Infrastructure constructor necessary to load a <b>BindableDomainObject</b> from a datasource.
  /// </summary>
  /// <remarks>
  /// All derived classes have to implement an (empty) constructor with this signature.
  /// Do not implement any initialization logic in this constructor, but use <see cref="DomainObject.OnLoaded"/> instead.
  /// </remarks>
  /// <param name="dataContainer">The newly loaded <b>DataContainer</b></param>
  protected BindableDomainObject (DataContainer dataContainer) : base (dataContainer)
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  // methods and properties

  /// <summary>
  /// Returns the <see cref="IBusinessObjectProperty"/> for a given <i>propertyIdentifier</i>.
  /// </summary>
  /// <param name="propertyIdentifier">The name of the property to return, as specified in the mapping file.</param>
  /// <returns>An instance of <see cref="BaseProperty"/> or a derived class, depending on the <see cref="System.Type"/> of the propery.</returns>
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  public IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  /// <summary>
  /// Gets a <see cref="DomainObjectClass"/> representing the <see cref="BindableDomainObject"/>.
  /// </summary>
  // TODO Doc: exceptions
  [EditorBrowsable (EditorBrowsableState.Never)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return new DomainObjectClass (this.GetType()); }
  }

  /// <summary>
  /// Gets or sets the value of a given <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><i>property</i> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="InvalidNullAssignmentException"><i>value</i> is a null reference, which is not valid for the property.</exception>
  /// <exception cref="ArgumentException"><i>value</i> has an invalid type for the property.</exception>
  // returns null if it is equal to the MinValue of the type
  public object this [IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  /// <summary>
  /// Gets or sets the value of the property with a given name.
  /// </summary>
  /// <value>The value of the property. Must not be <see langword="null"/>.</value>
  /// <param name="property">The property identifier of the property to return.</param>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is a null reference.</exception>
  /// <exception cref="InvalidNullAssignmentException"><i>value</i> is a null reference, which is not valid for the property.</exception>
  /// <exception cref="ArgumentException"><i>value</i> is of a type that is incompatible for the <i>property</i>.</exception>
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  public object this [string property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  // TODO Doc: 
  public string GetPropertyString (string property)
  {
    return GetPropertyString (GetBusinessObjectProperty (property));
  }

  // TODO Doc: 
  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  // TODO Doc: 
  public virtual string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return _objectReflector.GetPropertyString (property, format);
  }

  /// <summary>
  /// Returns the value of a given <i>property</i>.
  /// </summary>
  /// <param name="property">The property identifier of the property to return. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is a null reference.</exception>
  // TODO: throws an ArgumentNullException if the property with the given name does not exist. Throw better exception
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  public object GetProperty (string property)
  {
    return GetProperty (GetBusinessObjectProperty (property));
  }

  /// <summary>
  /// Returns the value of a given <i>property</i>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><i>property</i> is not derived from <see cref="BaseProperty"/>.</exception>
  public object GetProperty (IBusinessObjectProperty property)
  {
    return _objectReflector.GetProperty (property);
  }

  /// <summary>
  /// Sets the value of a given <i>property</i>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <param name="value">The new value for the property.</param>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentException"><i>value</i> is of a type that is incompatible for the <i>property</i>.</exception>
  // TODO: throws an ArgumentNullException if the property with the given name does not exist. Throw better exception
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  public void SetProperty (string property, object value)
  {
    SetProperty (GetBusinessObjectProperty (property), value);
  }

  /// <summary>
  /// Sets the value of a given <i>property</i>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <param name="value">The new value for the property.</param>
  /// <exception cref="System.ArgumentNullException"><i>property</i> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><i>property</i> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="ArgumentException"><i>value</i> is of a type that is incompatible for the <i>property</i>.</exception>
  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    _objectReflector.SetProperty (property, value);
  }

  /// <summary>
  /// Gets the <see cref="DomainObject.ID"/> of the <b>BindableDomainObject</b> as a string.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual string DisplayName 
  { 
    get { return ID.ToString (); } 
  }

  /// <summary>
  /// Gets the <see cref="DomainObject.ID"/> of the <b>BindableDomainObject</b> as a string.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  string IBusinessObjectWithIdentity.UniqueIdentifier
  {
    get { return ID.ToString (); }
  }
}
}
