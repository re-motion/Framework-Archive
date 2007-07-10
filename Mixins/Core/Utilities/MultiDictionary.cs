using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubicon.Mixins.Utilities
{
  // TODO: Remove when Rubicon.Collections.MultiDictionary is available
  [Serializable]
  internal class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
  {
    private Dictionary<TKey, List<TValue>> _innerDictionary;
    private int _count = 0;

    public MultiDictionary ()
    {
      _innerDictionary = new Dictionary<TKey, List<TValue>> ();
    }

    public MultiDictionary (IEqualityComparer<TKey> comparer)
    {
      _innerDictionary = new Dictionary<TKey, List<TValue>> (comparer);
    }

    public IEnumerable<TKey> Keys
    {
      get { return _innerDictionary.Keys; }
    }

    public int Count
    {
      get { return _count; }
    }

    public void Add (TKey key, TValue value)
    {
      GetOrCreateValueList (key).Add (value);
      ++_count;
    }

    public void Clear ()
    {
      _innerDictionary.Clear();
      _count = 0;
    }

    private List<TValue> GetOrCreateValueList (TKey key)
    {
      if (!_innerDictionary.ContainsKey (key))
        _innerDictionary.Add (key, new List<TValue> ());
      return _innerDictionary[key];
    }

    public bool ContainsKey (TKey key)
    {
      return _innerDictionary.ContainsKey (key);
    }

    public int GetValueCount (TKey key)
    {
      if (_innerDictionary.ContainsKey (key))
        return _innerDictionary[key].Count;
      else
        return 0;
    }

    public IEnumerable<TValue> this[TKey key]
    {
      get { return _innerDictionary[key]; }
    }

    public TValue GetFirstValue (TKey key)
    {
      return _innerDictionary[key][0];
    }

    public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
    {
      return _innerDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}