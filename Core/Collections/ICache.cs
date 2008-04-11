using System;

namespace Remotion.Collections
{
  /// <summary>
  /// Provides a comnmon interface for caches, which provide efficient storage and retrieval for values that are costly to calculate.
  /// </summary>
  /// <typeparam name="TKey">The key type via which values should be indexed.</typeparam>
  /// <typeparam name="TValue">The type of the values to be stored in the cache.</typeparam>
  /// <remarks>
  /// Caches are only meant for performance improvement, they are not reliable data containers. Do not rely on values being present in the cache;
  /// caches might choose to remove individual items (or all their items) at any time.
  /// </remarks>
  public interface ICache<TKey, TValue> : INullObject
  {
    [Obsolete ("Add is not safe for all caches (can lead to race conditions). Try using GetOrCreateValue() instead.", false)]
    void Add (TKey key, TValue value);
    TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory);

    bool TryGetValue (TKey key, out TValue value);

    void Clear();
  }
}

