using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A <see cref="DomainObject"/> that supports 2-way data binding of user controls.
/// </summary>
[Serializable]
public abstract class BindableDomainObject: DomainObject, IBusinessObjectWithIdentity, IDeserializationCallback
{
  // types

  // static members and constants

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that should be loaded.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <paramref name="id"/> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  // TODO: [Obsolete("Use DomainObject.GetObject<> instead.")]
  internal protected static new BindableDomainObject GetObject (ObjectID id)
  {
    return (BindableDomainObject) DomainObject.GetObject (id);
  }

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that should be loaded.</param>
  /// <param name="includeDeleted">Indicates if the method should return <b>BindableDomainObject</b>s that are already deleted.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <paramref name="id"/> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  // TODO: [Obsolete("Use DomainObject.GetObject<> instead.")]
  internal protected static new BindableDomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return (BindableDomainObject) DomainObject.GetObject (id, includeDeleted);
  }

  /// <summary>
  /// Gets a <b>BindableDomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>BindableDomainObject</b> that is loaded.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <b>BindableDomainObject</b>.</param>
  /// <returns>The <b>BindableDomainObject</b> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <paramref name="id"/> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  // TODO: [Obsolete("Use DomainObject.GetObject<> instead.")]
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
  /// <returns>The <b>BindableDomainObject</b> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="System.InvalidCastException">
  ///   The loaded object with the given <paramref name="id"/> cannot be casted to <b>BindableDomainObject</b>
  /// </exception>
  // TODO: [Obsolete("Use DomainObject.GetObject<> instead.")]
  internal protected static new BindableDomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return (BindableDomainObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  [NonSerialized]
  private BusinessObjectReflector _objectReflector;

  // construction and disposing

	/// <summary>
	/// Initializes a new <see cref="BindableDomainObject"/> with the current <see cref="DomainObjects.ClientTransaction"/>.
	/// </summary>
	/// <remarks>Any constructors implemented on concrete domain objects should delegate to this base constructor, apart from the infrastructure
	/// constructor (see <see cref="BindableDomainObject(DataContainer)"/>). As domain objects generally should not be constructed via the
	/// <c>new</c> operator, these constructors should remain protected, and the concrete domain objects should have a static "NewObject" method,
	/// which delegates to <see cref="DomainObject.NewObject"/>, passing it the required constructor arguments.</remarks>
  protected BindableDomainObject ()
    : base ()
  {
    Initialize ();
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
    Initialize ();
  }

	#region Obsolete legacy constructor
  /// <summary>
  /// Initializes a new <see cref="BindableDomainObject"/>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <see cref="BindableDomainObject"/>
	/// should be part of. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> is <see langword="null"/>.</exception>
  [Obsolete ("This constructor is obsolete, use the BindableDomainObject() one in conjunction with CurrentTransactionScope instead.")]
	protected BindableDomainObject (ClientTransaction clientTransaction) : base (clientTransaction)
  {
    Initialize ();
  }

  #endregion

  private void Initialize ()
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  // methods and properties

  internal bool? HasPropertyStillDefaultValue (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    if (!DataContainer.PropertyValues.Contains (propertyName))
      return null;
    if (State == StateType.New)
      return false;
    return DataContainer.PropertyValues[propertyName].HasChanged;
  }

  #region IBusinessObject Members

  /// <summary>
  /// Gets a <see cref="DomainObjectClass"/> representing the <see cref="BindableDomainObject"/>.
  /// </summary>
  // TODO Doc: exceptions
  [StorageClassNone]
  IBusinessObjectClass IBusinessObject.BusinessObjectClass
  {
    get { return new DomainObjectClass (this.GetPublicDomainObjectType()); }
  }

  /// <summary>
  /// Gets or sets the value of a given <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="InvalidNullAssignmentException"><paramref name="value"/> is <see langword="null"/>, which is not valid for the property.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid type for the property.</exception>
  // TODO Doc: returns null if it is equal to the MinValue of the type
  [StorageClassNone]
  object IBusinessObject.this[IBusinessObjectProperty property]
  {
    get { return ((IBusinessObject) this).GetProperty (property); }
    set { ((IBusinessObject) this).SetProperty (property, value); }
  }

  /// <summary>
  /// Gets or sets the value of the property with a given name.
  /// </summary>
  /// <value>The value of the property. Must not be <see langword="null"/>.</value>
  /// <param name="property">The property identifier of the property to return.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="InvalidNullAssignmentException"><paramref name="value"/> is <see langword="null"/>, which is not valid for the property.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> is of a type that is incompatible for the <paramref name="property"/>.</exception>
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  [StorageClassNone]
  object IBusinessObject.this[string property]
  {
    get { return ((IBusinessObject) this).GetProperty (property); }
    set { ((IBusinessObject) this).SetProperty (property, value); }
  }

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The name of the requested property.</param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  string IBusinessObject.GetPropertyString (string property)
  {
    return ((IBusinessObject) this).GetPropertyString (GetBusinessObjectProperty (property));
  }

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The requested property.</param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  string IBusinessObject.GetPropertyString (IBusinessObjectProperty property)
  {
    return ((IBusinessObject) this).GetPropertyString (property, null);
  }

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The requested property.</param>
  /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  string IBusinessObject.GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return _objectReflector.GetPropertyString (property, format);
  }

  /// <summary>
  /// Returns the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property identifier of the property to return. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  // TODO: throws an ArgumentNullException if the property with the given name does not exist. Throw better exception
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  object IBusinessObject.GetProperty (string property)
  {
    return ((IBusinessObject) this).GetProperty (GetBusinessObjectProperty (property));
  }

  /// <summary>
  /// Returns the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  object IBusinessObject.GetProperty (IBusinessObjectProperty property)
  {
    return _objectReflector.GetProperty (property);
  }

  /// <summary>
  /// Sets the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <param name="value">The new value for the property.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> is of a type that is incompatible for the <paramref name="property"/>.</exception>
  // TODO: throws an ArgumentNullException if the property with the given name does not exist. Throw better exception
  // TODO Doc: exceptions
  // all exceptions from GetBusinessObjectProperty
  void IBusinessObject.SetProperty (string property, object value)
  {
    ((IBusinessObject) this).SetProperty (GetBusinessObjectProperty (property), value);
  }

  /// <summary>
  /// Sets the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <param name="value">The new value for the property.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> is of a type that is incompatible for the <paramref name="property"/>.</exception>
  void IBusinessObject.SetProperty (IBusinessObjectProperty property, object value)
  {
    _objectReflector.SetProperty (property, value);
  }

  #endregion

  #region IBusinessObjectWithIdentity Members

  /// <summary>
  /// Gets the <see cref="DomainObject.ID"/> of the <b>BindableDomainObject</b> as a string.
  /// </summary>
  [StorageClassNone]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual string DisplayName
  {
    get { return ID.ToString (); }
  }

  /// <summary>
  ///   Gets the value of <see cref="DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
  ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
  /// </summary>
  [StorageClassNone]
  string IBusinessObjectWithIdentity.DisplayNameSafe
  {
    get
    {
      IBusinessObjectClass businessObjectClass = ((IBusinessObject) this).BusinessObjectClass;
      IBusinessObjectProperty displayNameProperty = businessObjectClass.GetPropertyDefinition ("DisplayName");
      if (displayNameProperty.IsAccessible (businessObjectClass, this))
        return DisplayName;

      return businessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();
    }
  }

  /// <summary>
  /// Gets the <see cref="DomainObject.ID"/> of the <b>BindableDomainObject</b> as a string.
  /// </summary>
  [StorageClassNone]
  string IBusinessObjectWithIdentity.UniqueIdentifier
  {
    get { return ID.ToString (); }
  }

  #endregion

  #region IDeserializationCallback Members

  // TODO Doc:
  void IDeserializationCallback.OnDeserialization (object sender)
  {
    OnDeserialization (sender);
  }

  // TODO Doc:
  protected virtual void OnDeserialization (object sender)
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  #endregion

  private IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return ((IBusinessObject) this).BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }
}
}
