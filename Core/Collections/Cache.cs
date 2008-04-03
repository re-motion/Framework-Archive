using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Collections
{
  //TODO: Doc
  [Serializable]
  public class Cache<TKey, TValue> : ICache<TKey, TValue>
  {
    // types

    // static members

    // member fields

    private Dictionary<TKey, TValue> _cache = new Dictionary<TKey,TValue>();

    // construction and disposing

    public Cache ()
    {
    }

    // methods and properties

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      _cache[key] = value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      
      return _cache.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      _cache.Clear ();
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      TValue value;
      if (!_cache.TryGetValue (key, out value))
      {
        value = valueFactory (key);
        _cache[key] = value;
      }

      return value;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}