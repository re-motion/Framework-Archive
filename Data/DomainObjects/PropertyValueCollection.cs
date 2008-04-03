using System;
using System.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// A typed collection for <see cref="PropertyValue"/> objects.
/// </summary>
public class PropertyValueCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  /// <summary>
  /// Occurs before the <see cref="PropertyValue.Value"/> of a <see cref="PropertyValue"/> in the <see cref="PropertyValueCollection"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanging;
  /// <summary>
  /// Occurs after the <see cref="PropertyValue.Value"/> of a <see cref="PropertyValue"/> in the <see cref="PropertyValueCollection"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanged;

  private DataContainer _dataContainer;
  private bool _isDiscarded = false;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>PropertyValueCollection</b> object.
  /// </summary>
  public PropertyValueCollection ()
  {
  }

  /// <summary>
  /// Initializes a new <b>PropertyValueCollection</b> as a shallow copy of a given <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <remarks>
  /// The new <b>PropertyValueCollection</b> has the same items as the given <paramref name="collection"/>.
  /// </remarks>
  /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy. Must not be <see langword="null"/>.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
  // standard constructor for collections
  public PropertyValueCollection (PropertyValueCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (PropertyValue propertyValue in collection)
    {
      Add (propertyValue);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  #region Standard implementation for "add-only" collections

  /// <summary>
  /// Determines whether the <see cref="PropertyValueCollection"/> contains a specific <see cref="PropertyValue"/>.
  /// </summary>
  /// <param name="propertyValue">The object to locate in the <see cref="PropertyValueCollection"/>. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if the <see cref="PropertyValueCollection"/> contains the <paramref name="propertyValue"/>; otherwise <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyValue"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
  public bool Contains (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
    CheckDiscarded ();

    return BaseContains (propertyValue.Name, propertyValue);
  }

  /// <summary>
  /// Determines whether the <see cref="PropertyValueCollection"/> contains a specific property name.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> to locate in the <see cref="PropertyValueCollection"/>. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if the <see cref="PropertyValueCollection"/> contains the key; otherwise <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return BaseContainsKey (propertyName);
  }

  /// <summary>
  /// Gets the <see cref="PropertyValue"/> with a given <paramref name="index"/> in the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public PropertyValue this [int index]  
  {
    get 
    { 
      CheckDiscarded ();
      return (PropertyValue) BaseGetObject (index); 
    }
  }

  /// <summary>
  /// Gets the <see cref="PropertyValue"/> with a given <paramref name="propertyName"/> in the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the collection.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public PropertyValue this [string propertyName]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckDiscarded ();

      if (!Contains (propertyName))
      {
        throw CreateArgumentException ("Property '{0}' does not exist.", "propertyName", propertyName);
      }
  
      return (PropertyValue) BaseGetObject (propertyName); 
    }
  }

  /// <summary>
  /// Adds a <see cref="PropertyValue"/> to the collection.
  /// </summary>
  /// <param name="value">The <see cref="PropertyValue"/> to add. Must not be <see langword="null"/>.</param>
  /// <returns>The position into which the <see cref="PropertyValue"/> was inserted.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentException"><paramref name="value"/> is already part of the collection.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public int Add (PropertyValue value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
    CheckDiscarded ();

    if (Contains (value.Name))
      throw CreateArgumentException ("Property '{0}' already exists in collection.", "value", value.Name);

    int position = BaseAdd (value.Name, value);
    value.RegisterForAccessObservation (this);

    return position;
  }

  #endregion

  /// <summary>
  /// Copies the items of the <see cref="PropertyValueCollection"/> to an array, starting at a particular array index.
  /// </summary>
  /// <param name="array">The one-dimensional array that is the destination of the items copied from <see cref="PropertyValueCollection"/>. The array must have zero-based indexing. Must not be <see langword="null"/>.</param>
  /// <param name="index">The zero-based index in array at which copying begins.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is smaller than 0.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="array"/> is not a one-dimensional array.<br /> -or- <br />
  ///   <paramref name="index"/> is greater than the current length of the array.<br /> -or- <br />
  ///   The number of items is greater than the available space from <paramref name="index"/> to the end of <paramref name="array"/>.
  /// </exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override void CopyTo (Array array, int index)
  {
    CheckDiscarded ();
    base.CopyTo (array, index);
  }

  /// <summary>
  /// Gets the number of items contained in the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override int Count
  {
    get
    {
      CheckDiscarded ();
      return base.Count;
    }
  }

  /// <summary>
  /// Returns an enumerator that can iterate through the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <returns>An <see cref="System.Collections.IEnumerator"/> for the entire <see cref="PropertyValueCollection"/>.</returns>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override IEnumerator GetEnumerator ()
  {
    CheckDiscarded ();
    return base.GetEnumerator ();
  }

  /// <summary>
  /// Gets a value indicating whether the <see cref="CommonCollection"/> is read-only.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override bool IsReadOnly
  {
    get
    {
      CheckDiscarded ();
      return base.IsReadOnly;
    }
  }

  /// <summary>
  /// Gets a value indicating whether access to the <see cref="PropertyValueCollection"/> is synchronized (thread-safe).
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override bool IsSynchronized
  {
    get
    {
      CheckDiscarded ();
      return base.IsSynchronized;
    }
  }

  /// <summary>
  /// Gets an object that can be used to synchronize access to the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override object SyncRoot
  {
    get
    {
      CheckDiscarded ();
      return base.SyncRoot;
    }
  }

  /// <summary>
  /// Gets a value indicating the discarded status of the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <remarks>
  /// For more information why and when a <see cref="PropertyValueCollection"/> is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanging (PropertyChangeEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanged (PropertyChangeEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  internal void Discard ()
  {
    foreach (PropertyValue propertyValue in this)
      propertyValue.Discard ();

    _isDiscarded = true;
  }

  internal void RegisterForChangeNotification (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    if (_dataContainer != null) throw new InvalidOperationException ("PropertyValueCollection already has a DataContainer set.");

    _dataContainer = dataContainer;
  }

  internal void PropertyValueChanging (PropertyValue propertyValue, ValueChangeEventArgs e)
  {
    PropertyChangeEventArgs eventArgs = new PropertyChangeEventArgs (propertyValue, e.OldValue, e.NewValue);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of DataContainer when changing property values is not organized through events.
    if (_dataContainer != null)
      _dataContainer.PropertyValueChanging (this, eventArgs);

    OnPropertyChanging (eventArgs);
  }

  internal void PropertyValueChanged (PropertyValue propertyValue, ValueChangeEventArgs e)
  {
    PropertyChangeEventArgs eventArgs = new PropertyChangeEventArgs (propertyValue, e.OldValue, e.NewValue);
    OnPropertyChanged (eventArgs);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of DataContainer when changing property values is not organized through events.
    if (_dataContainer != null)
      _dataContainer.PropertyValueChanged (this, eventArgs);
  }

  internal void PropertyValueReading (PropertyValue propertyValue, ValueAccess valueAccess)
  {
    if (_dataContainer != null)
      _dataContainer.PropertyValueReading (propertyValue, valueAccess);
  }

  internal void PropertyValueRead (PropertyValue propertyValue, object value, ValueAccess valueAccess)
  {
    if (_dataContainer != null)
      _dataContainer.PropertyValueRead (propertyValue, value, valueAccess);
  }

  private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }

  private void CheckDiscarded ()
  {
    if (_isDiscarded)
      throw new ObjectDiscardedException ();
  }
}
}
