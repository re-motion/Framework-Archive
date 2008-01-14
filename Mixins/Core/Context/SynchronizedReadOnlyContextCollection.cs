using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class SynchronizedReadOnlyContextCollection<TKey, TValue> : ICollection<TValue>, ICollection
  {
    private readonly object _syncObject = new object();
    private readonly ReadOnlyContextCollection<TKey, TValue> _internalCollection;

    public SynchronizedReadOnlyContextCollection (Func<TValue, TKey> keyGenerator, IEnumerable<TValue> values)
    {
      ArgumentUtility.CheckNotNull ("keyGenerator", keyGenerator);
      _internalCollection = new ReadOnlyContextCollection<TKey, TValue> (keyGenerator, values);
    }

    public int Count
    {
      get
      {
        lock (_syncObject)
        {
          return _internalCollection.Count;
        }
      }
    }

    public bool Contains (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      lock (_syncObject)
      {
        return _internalCollection.Contains (key);
      }
    }

    public bool Contains (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      lock (_syncObject)
      {
        return _internalCollection.Contains (value);
      }
    }

    public IEnumerator<TValue> GetEnumerator ()
    {
      lock (_syncObject)
      {
        return _internalCollection.GetEnumerator();
      }
    }

    public void CopyTo (TValue[] array, int arrayIndex)
    {
      lock (_syncObject)
      {
        _internalCollection.CopyTo (array, arrayIndex);
      }
    }

    void ICollection<TValue>.Add (TValue item)
    {
      lock (_syncObject)
      {
        ((ICollection<TValue>)_internalCollection).Add (item);
      }
    }

    void ICollection<TValue>.Clear ()
    {
      lock (_syncObject)
      {
        ((ICollection<TValue>) _internalCollection).Clear ();
      }
    }

    bool ICollection<TValue>.Remove (TValue item)
    {
      lock (_syncObject)
      {
        return ((ICollection<TValue>) _internalCollection).Remove (item);
      }
    }

    bool ICollection<TValue>.IsReadOnly
    {
      get
      {
        lock (_syncObject)
        {
          return ((ICollection<TValue>) _internalCollection).IsReadOnly;
        }
      }
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection) _internalCollection).CopyTo (array, index);
    }

    object ICollection.SyncRoot
    {
      get { return _syncObject; }
    }

    bool ICollection.IsSynchronized
    {
      get { return true; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      lock (_syncObject)
      {
        return ((IEnumerable) _internalCollection).GetEnumerator();
      }
    }
  }
}