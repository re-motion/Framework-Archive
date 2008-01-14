using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class ReadOnlyContextCollection<TKey, TValue> : ICollection<TValue>, ICollection
  {
    private readonly Func<TValue, TKey> _keyGenerator;
    private readonly IDictionary<TKey, TValue> _internalCollection;

    public ReadOnlyContextCollection (Func<TValue, TKey> keyGenerator, IEnumerable<TValue> values)
    {
      ArgumentUtility.CheckNotNull ("keyGenerator", keyGenerator);
      _internalCollection = new Dictionary<TKey, TValue>();
      _keyGenerator = keyGenerator;

      foreach (TValue value in values)
      {
        ArgumentUtility.CheckNotNull ("values[" + _internalCollection.Count + "]", value);
        _internalCollection.Add (_keyGenerator (value), value);
      }
    }

    public virtual int Count
    {
      get { return _internalCollection.Count; }
    }

    public virtual bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _internalCollection.ContainsKey (key);
    }

    public virtual bool Contains (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      TKey key = _keyGenerator (value);
      TValue foundValue;
      if (!_internalCollection.TryGetValue (key, out foundValue))
        return false;
      else
        return value.Equals (foundValue);
    }

    public virtual IEnumerator<TValue> GetEnumerator ()
    {
      return _internalCollection.Values.GetEnumerator();
    }

    public virtual void CopyTo (TValue[] array, int arrayIndex)
    {
      _internalCollection.Values.CopyTo (array, arrayIndex);
    }

    void ICollection<TValue>.Add (TValue item)
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    void ICollection<TValue>.Clear ()
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    bool ICollection<TValue>.Remove (TValue item)
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    bool ICollection<TValue>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection) _internalCollection.Values).CopyTo (array, index);
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_internalCollection).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}