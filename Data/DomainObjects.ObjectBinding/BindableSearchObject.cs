using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A class that can be used to store search paramers and supports 2-way data binding of user controls.
/// </summary>
[Serializable]
public abstract class BindableSearchObject : IBusinessObject, IDeserializationCallback
{
  [NonSerialized]
  private BusinessObjectReflector _objectReflector;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
	public BindableSearchObject()
	{
    _objectReflector = new BusinessObjectReflector (this);
  }

  /// <summary>
  /// Returns the <see cref="IBusinessObjectProperty"/> for a given <paramref name="propertyIdentifier"/>.
  /// </summary>
  /// <param name="propertyIdentifier">The name of the property to return.</param>
  /// <returns>An instance of <see cref="BaseProperty"/> or a derived class, depending on the <see cref="System.Type"/> of the propery.</returns>
  // TODO Doc: exceptions
  public IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  /// <summary>
  /// Creates a Query with all search parameters set.
  /// </summary>
  public abstract IQuery CreateQuery ();

  #region IBusinessObject Members

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The name of the requested property.</param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  public string GetPropertyString (string property)
  {
    return GetPropertyString (GetBusinessObjectProperty (property));
  }

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The requested property.</param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  /// <summary>
  /// Gets the string representation of the value of the specified <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The requested property.</param>
  /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
  /// <returns>A string representing the value of the given <paramref name="property"/></returns>
  // TODO Doc: Exceptions
  public string GetPropertyString (IBusinessObjectProperty property, string format)
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
  public object GetProperty (string property)
  {
    return GetProperty (GetBusinessObjectProperty (property));
  }

  /// <summary>
  /// Returns the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  public object GetProperty (IBusinessObjectProperty property)
  {
    return _objectReflector.GetProperty (property);
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
  public object this[string property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  /// <summary>
  /// Gets or sets the value of a given <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="InvalidNullAssignmentException"><paramref name="value"/> is <see langword="null"/>, which is not valid for the property.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid type for the property.</exception>
  // TODO Doc: returns null if it is equal to the MinValue of the type
  public object this[IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
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
  public void SetProperty (string property, object value)
  {
    SetProperty (GetBusinessObjectProperty (property), value);
  }

  /// <summary>
  /// Sets the value of a given <paramref name="property"/>.
  /// </summary>
  /// <param name="property">The property to return. Must not be <see langword="null"/>.</param>
  /// <param name="value">The new value for the property.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentTypeException"><paramref name="property"/> is not derived from <see cref="BaseProperty"/>.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> is of a type that is incompatible for the <paramref name="property"/>.</exception>
  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    _objectReflector.SetProperty (property, value);
  }

  /// <summary>
  /// Gets a <see cref="DomainObjectClass"/> representing the <see cref="BindableDomainObject"/>.
  /// </summary>
  // TODO Doc: exceptions
  [EditorBrowsable (EditorBrowsableState.Never)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return new SearchObjectClass (this.GetType()); }
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
}
}
