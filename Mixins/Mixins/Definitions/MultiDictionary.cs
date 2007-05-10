using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mixins.Definitions
{
  [Serializable]
  internal class MultiDictionary<TKey, TValue>
  {
    private Dictionary<TKey, List<TValue>> _innerDictionary = new Dictionary<TKey, List<TValue>> ();

    public void Add (TKey key, TValue value)
    {
      GetOrCreateValueList (key).Add (value);
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
  }
}