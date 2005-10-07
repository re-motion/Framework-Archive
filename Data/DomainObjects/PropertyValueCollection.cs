using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
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
  public event PropertyChangingEventHandler PropertyChanging;
  /// <summary>
  /// Occurs after the <see cref="PropertyValue.Value"/> of a <see cref="PropertyValue"/> in the <see cref="PropertyValueCollection"/> is changed.
  /// </summary>
  public event PropertyChangedEventHandler PropertyChanged;

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
  /// The new <b>PropertyValueCollection</b> has the same items as the given <i>collection</i>.
  /// </remarks>
  /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy. Must not be <see langword="null"/>.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>collection</i> is <see langword="null"/>.</exception>
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
  /// <returns><b>true</b> if the <see cref="PropertyValueCollection"/> contains the key; otherwise <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyValue</i> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool Contains (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
    CheckDiscarded ();

    return Contains (propertyValue.Name);
  }

  /// <summary>
  /// Determines whether the <see cref="PropertyValueCollection"/> contains a specific property name.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> to locate in the <see cref="PropertyValueCollection"/>. Must not be <see langword="null"/>.</param>
  /// <returns><b>true</b> if the <see cref="PropertyValueCollection"/> contains the key; otherwise <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return BaseContainsKey (propertyName);
  }

  /// <summary>
  /// Gets the <see cref="PropertyValue"/> with a given <i>index</i> in the <see cref="PropertyValueCollection"/>.
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
  /// Gets the <see cref="PropertyValue"/> with a given <i>propertyName</i> in the <see cref="PropertyValueCollection"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <i>propertyName</i> does not exist in the collection.</exception>
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
  /// <exception cref="ArgumentNullException"><i>value</i> is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentException"><i>value</i> is already part of the collection.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public int Add (PropertyValue value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
    CheckDiscarded ();

    if (Contains (value.Name))
      throw CreateArgumentException ("Property '{0}' already exists in collection.", "value", value.Name);

    value.Changing += new ValueChangingEventHandler (PropertyValue_Changing);
    value.Changed += new EventHandler (PropertyValue_Changed);
    return BaseAdd (value.Name, value);
  }

  #endregion

  /// <summary>
  /// Copies the items of the <see cref="PropertyValueCollection"/> to an array, starting at a particular array index.
  /// </summary>
  /// <param name="array">The one-dimensional array that is the destination of the items copied from <see cref="PropertyValueCollection"/>. The array must have zero-based indexing. Must not be <see langword="null"/>.</param>
  /// <param name="index">The zero-based index in array at which copying begins.</param>
  /// <exception cref="System.ArgumentNullException"><i>array</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>index</i> is smaller than 0.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>array</i> is not a one-dimensional array.<br /> -or- <br />
  ///   <i>index</i> is greater than the current length of the array.<br /> -or- <br />
  ///   The number of items is greater than the available space from <i>index</i> to the end of <i>array</i>.
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
  /// For more information why and when a <see cref="PropertyValueCollection"/> is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanging (PropertyChangingEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanged (PropertyChangedEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  internal void Discard ()
  {
    foreach (PropertyValue propertyValue in this)
    {
      propertyValue.Changing -= new ValueChangingEventHandler (PropertyValue_Changing);
      propertyValue.Changed -= new EventHandler (PropertyValue_Changed);

      propertyValue.Discard ();
    }

    _isDiscarded = true;
  }

  private void PropertyValue_Changing (object sender, ValueChangingEventArgs e)
  {
    PropertyChangingEventArgs eventArgs = new PropertyChangingEventArgs (
        (PropertyValue) sender, e.OldValue, e.NewValue);

    OnPropertyChanging (eventArgs);
  }

  private void PropertyValue_Changed (object sender, EventArgs e)
  {
    OnPropertyChanged (new PropertyChangedEventArgs ((PropertyValue) sender));
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
