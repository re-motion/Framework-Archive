using System;
using System.Collections;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//TODO documentation: comment this class
public class CollectionBase : ICollection
{
  // types

  private class CollectionEnumerator : IEnumerator, IDisposable
  {
    private int _index;
    private long _collectionVersion;
    private CollectionBase _collection;

    public CollectionEnumerator (CollectionBase collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      _collection = collection;
      _collectionVersion = _collection._version;
      _index = -1;
    }

    #region IEnumerator Members

    public void Reset ()
    {
      CheckVersion ();
      _index = -1;
    }

    public object Current
    {
      get { return _collection.GetObject (_index); }
    }

    public bool MoveNext ()
    {
      CheckVersion ();
      _index++;
      return (_index < _collection.Count);
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      _collection = null;
    }

    #endregion

    private void CheckVersion ()
    {
      if (!_collection.IsReadOnly && _collectionVersion != _collection._version)
        throw new InvalidOperationException ("Collection was modified during enumeration.");
    }
  }


  // static members and constants

  // member fields

  private Hashtable _collectionData;
  private ArrayList _collectionKeys;
  private bool _isReadOnly;
  private long _version;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>ColletionBase</b>.
  /// </summary>
  protected CollectionBase ()
  {
    _collectionData = new Hashtable ();
    _collectionKeys = new ArrayList ();
  }

  // methods and properties

//TODO documentation: ArgumentOutOfRangeException?
//TODO documentation: ArgumentNullException?
  /// <summary>
  /// Returns the object with a given index from the collection 
  /// </summary>
  /// <param name="index">The index of the object to return.</param>
  /// <returns>The object with the given index.</returns>
  protected object GetObject (int index)
  {
    return _collectionData[_collectionKeys[index]];
  }

//TODO documentation: ArgumentOutOfRangeException?
  /// <summary>
  /// Returns the object with a given key from the collection.
  /// </summary>
  /// <param name="key">The key of the object to return.</param>
  /// <returns>The object with the given key.</returns>
  /// <exception cref="ArgumentNullException"><i>key</i> is a null reference.</exception>
  protected object GetObject (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);

    return _collectionData[key];
  }

  /// <summary>
  /// Gets a value indicating whether the <see cref="CollectionBase"/> is read-only.
  /// </summary>
  public virtual bool IsReadOnly
  {
    get { return _isReadOnly; }
  }

  /// <summary>
  /// Determines whether the <see cref="CollectionBase"/> contains a specific key.
  /// </summary>
  /// <param name="key">The key to locate in the <see cref="CollectionBase"/>.</param>
  /// <returns><b>true</b> if the <see cref="CollectionBase"/> contains the key; otherwise <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>key</i> is a null reference.</exception>
  protected bool ContainsKey (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);

    return _collectionData.ContainsKey (key);
  }

//TODO documentation: check param description
//TODO: Would an InvalidOperationException be more appropriate than a NotSupportedException?
  /// <summary>
  /// Adds an object to the collection with a specified key.
  /// </summary>
  /// <param name="key">The key of the object to add.</param>
  /// <param name="value">The object to add.</param>
  /// <exception cref="System.NotSupportedException">The collection is read only.</exception>
  protected void Add (object key, object value)
  {
    ArgumentUtility.CheckNotNull ("key", key);
    ArgumentUtility.CheckNotNull ("value", value);
    if (_isReadOnly) throw new NotSupportedException ("Cannot add an element to a read-only collection.");

    _collectionData.Add (key, value);
    _collectionKeys.Add (key);
    _version++;
  }

//TODO: Would an InvalidOperationException be more appropriate than a NotSupportedException?
  /// <summary>
  /// Removes an object from the collection with a specified key.
  /// </summary>
  /// <param name="key">The key of the object to remove.</param>
  /// <exception cref="System.ArgumentNullException"><i>key</i> is a null reference.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read only.</exception>
  protected void Remove (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);
    if (_isReadOnly) throw new NotSupportedException ("Cannot remove an element from a read-only collection.");

    _collectionData.Remove (key);
    _collectionKeys.Remove (key);
    _version++;
  }

  /// <summary>
  /// Removes all objects from the collection.
  /// </summary>
  protected void ClearCollection ()
  {
    _collectionData.Clear ();
    _collectionKeys.Clear ();
    _version++;
  }

  /// <summary>
  /// Sets the <see cref="IsReadOnly"/> property of the collection.
  /// </summary>
  /// <param name="isReadOnly">The new value for the <see cref="IsReadOnly"/> property of the collection.</param>
  protected void SetIsReadOnly (bool isReadOnly)
  {
    _isReadOnly = isReadOnly;
  }

  private ArgumentException CreateArgumentException (string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args));
  }

  #region IEnumerable Members

  /// <summary>
  /// Returns an enumerator that can iterate through the <see cref="CollectionBase"/>.
  /// </summary>
  /// <returns></returns>
  public virtual IEnumerator GetEnumerator ()
  {
    return new CollectionEnumerator (this);
  }

  #endregion

  #region ICollection Members

  /// <summary>
  /// Gets a value indicating whether access to the <see cref="CollectionBase"/> is synchronized (thread-safe).
  /// </summary>
  public virtual bool IsSynchronized
  {
    get { return false; }
  }

  /// <summary>
  /// Gets the number of elements contained in the <see cref="CollectionBase"/>.
  /// </summary>
  public virtual int Count
  {
    get { return _collectionData.Count; }
  }

//TODO: ArgumentOutOfRange better suited for index >= array.Length?
  /// <summary>
  /// Copies the elements of the <see cref="CollectionBase"/> to an Array, starting at a particular Array index.
  /// </summary>
  /// <param name="array">The one-dimensional Array that is the destination of the elements copied from <see cref="CollectionBase"/>. The Array must have zero-based indexing.</param>
  /// <param name="index">The zero-based index in array at which copying begins.</param>
  /// <exception cref="System.ArgumentNullException"><i>array</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>index</i> is smaller than 0.</exception>
  /// <exception cref="System.ArgumentException">
  /// <para><i>array</i> is not a one-dimensional array.</para>
  /// <para><i>index</i> is greater than the current length of the array.</para>
  /// <para>The number of elements is greater than the available space from <i>index</i> to the end of <i>array</i>.</para>
  /// </exception>
  public virtual void CopyTo (Array array, int index)
  {
    ArgumentUtility.CheckNotNull ("array", array);
    if (index < 0) throw new ArgumentOutOfRangeException ("index", index, "Index must be greater than or equal to zero.");
    if (array.Rank != 1) throw new ArgumentException ("CopyTo can only operate on one-dimensional arrays.", "array");
    if (index >= array.Length) throw new ArgumentException ("Index cannot be equal to or greater than the length of the array.", "index");
    if ((array.Length - index) < Count) throw new ArgumentException ("The number of elements in the source collection is greater than the available space from index to the end of the destination array.", "index");

    for (int i = 0; i < Count; i++)
      array.SetValue (this.GetObject (i), index + i);
  }

  /// <summary>
  /// Gets an object that can be used to synchronize access to the <see cref="CollectionBase"/>.
  /// </summary>
  public virtual object SyncRoot
  {
    get { return this; }
  }

  #endregion
}
}
