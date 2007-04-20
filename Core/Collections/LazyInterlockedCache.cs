using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.Collections
{
  /// <summary>
  /// A thread-save cache with optimized interlocking behaviour. 
  /// </summary>
  /// <remarks>
  ///   The cache object is only locked while a new key is added to the cache. If a new value needs to be created, 
  ///   only its key will be locked during creation. For best performance, first try to get the object using
  ///   <see cref="TryGetValue"/>. If no object is found, call <see cref="GetOrCreateValue"/> using a factory
  ///   method that will be called if the key still cannot be found.
  /// </remarks>
  /// <typeparam name="TKey"> Type of the cache key. </typeparam>
  /// <typeparam name="TValue"> Type of the cache value. </typeparam>
  [Obsolete ("Experimental code. Stability and performance advantages questionable. Use SimpleInterlockedCache for production code. Both implement ICache")] 
  public class LazyInterlockedCache<TKey, TValue>: ICache<TKey, TValue>
  {
    private class ValueContainer
    {
      private object _syncRoot = new object();

      public TValue Value;
      public bool IsValid = true;

      public void WaitForRelease()
      {
        lock (_syncRoot)
        {
        }
      }

      public void Enter()
      {
        Monitor.Enter (_syncRoot);
      }

      public void Exit ()
      {
        Monitor.Exit (_syncRoot);
      }
    }

    private Dictionary<TKey, ValueContainer> _cache;

    public LazyInterlockedCache ()
      : this (null)
    {
    }

    public LazyInterlockedCache (IEqualityComparer<TKey> comparer)
    {
      _cache = new Dictionary<TKey, ValueContainer> (comparer);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ValueContainer valueContainer;
      value = default(TValue);

      bool success = _cache.TryGetValue (key, out valueContainer);
      if (! success)
        return false;

      valueContainer.WaitForRelease();
      if (! valueContainer.IsValid)
        return false;

      value = valueContainer.Value;
      return true;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      TValue value;
      if (TryGetValue (key, out value))
        return value;

      ValueContainer valueContainer = null;
      try
      {
        lock (_cache)
        {
          if (TryGetValue (key, out value))
            return value;

          valueContainer = new ValueContainer();
          valueContainer.Enter();
          _cache.Add (key, valueContainer);
        }

        try
        {
          valueContainer.Value = valueFactory (key);
        }
        catch
        {
          _cache.Remove (key);
          valueContainer.IsValid = false;
          throw;
        }
      }
      finally
      {
        if (valueContainer != null)
          valueContainer.Exit();
      }

      return valueContainer.Value;
    }

    public void Add (TKey key, TValue value)
    {
      ValueContainer container = new ValueContainer();
      container.Value = value;
      container.IsValid = true;
      lock (_cache)
        _cache.Add (key, container);
    }

    public void Clear()
    {
      lock (_cache)
      {
        _cache.Clear();
      }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
