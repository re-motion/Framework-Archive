using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// A simple thread-safe cache.
  /// </summary>
  [Serializable]
  public class InterlockedCache<TKey, TValue> : ICache<TKey, TValue>
  {
    private Dictionary<TKey, TValue> _cache;

    public InterlockedCache ()
      : this (null)
    {
    }

    public InterlockedCache (IEqualityComparer<TKey> comparer)
    {
      _cache = new Dictionary<TKey,TValue> (comparer);
    }

    [Obsolete ("Add is not safe for interlocked caches (can lead to race conditions). Use GetOrCreateValue() instead.", false)]
    public void Add (TKey key, TValue value)
    {
      // throw new NotSupportedException ("Add is not safe for interlocked caches.");
      ArgumentUtility.CheckNotNull ("key", key);

      lock (_cache)
        _cache.Add (key, value);
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      lock (_cache)
      {
        TValue value;
        if (! _cache.TryGetValue (key, out value))
        {
          value = valueFactory (key);
          _cache.Add (key, value);
        }
        return value;
      }
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      lock (_cache)
        return _cache.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      lock (_cache)
        _cache.Clear ();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
