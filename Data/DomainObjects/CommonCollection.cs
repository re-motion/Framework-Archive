using System;
using System.Collections;

namespace Rubicon.Data.DomainObjects
{
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

  protected CollectionBase ()
  {
    _collectionData = new Hashtable ();
    _collectionKeys = new ArrayList ();
  }

  // methods and properties

  internal protected object GetObject (int index)
  {
    return _collectionData[_collectionKeys[index]];
  }

  protected object GetObject (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);

    return _collectionData[key];
  }

  public bool IsReadOnly
  {
    get { return _isReadOnly; }
  }

  protected bool ContainsKey (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);

    return _collectionData.ContainsKey (key);
  }

  protected void Add (object key, object value)
  {
    ArgumentUtility.CheckNotNull ("key", key);
    ArgumentUtility.CheckNotNull ("value", value);
    if (_isReadOnly) throw new NotSupportedException ("Cannot add an element to a read-only collection.");

    _collectionData.Add (key, value);
    _collectionKeys.Add (key);
    _version++;
  }

  protected void Remove (object key)
  {
    ArgumentUtility.CheckNotNull ("key", key);
    if (_isReadOnly) throw new NotSupportedException ("Cannot remove an element from a read-only collection.");

    _collectionData.Remove (key);
    _collectionKeys.Remove (key);
    _version++;
  }

  protected void ClearCollection ()
  {
    _collectionData.Clear ();
    _collectionKeys.Clear ();
    _version++;
  }

  protected void SetIsReadOnly (bool isReadOnly)
  {
    _isReadOnly = isReadOnly;
  }

  private ArgumentException CreateArgumentException (string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args));
  }

  #region IEnumerable Members

  public IEnumerator GetEnumerator ()
  {
    return new CollectionEnumerator (this);
  }

  #endregion

  #region ICollection Members

  public bool IsSynchronized
  {
    get { return false; }
  }

  public int Count
  {
    get { return _collectionData.Count; }
  }

  public void CopyTo (Array array, int index)
  {
    ArgumentUtility.CheckNotNull ("array", array);
    if (index < 0) throw new ArgumentOutOfRangeException ("index", index, "Index must be greater than or equal to zero.");
    if (array.Rank != 1) throw new ArgumentException ("CopyTo can only operate on one-dimensional arrays.", "array");
    if (index >= array.Length) throw new ArgumentException ("Index cannot be equal to or greater than the length of the array.", "index");
    if ((array.Length - index) < Count) throw new ArgumentException ("The number of elements in the source collection is greater than the available space from index to the end of the destination array.", "index");

    for (int i = 0; i < Count; i++)
      array.SetValue (this.GetObject (i), index + i);
  }

  public object SyncRoot
  {
    get { return this; }
  }

  #endregion
}
}
