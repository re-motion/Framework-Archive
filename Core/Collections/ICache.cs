using System;

namespace Remotion.Collections
{
  //TODO: Doc
  public interface ICache<TKey, TValue> : INullObject
  {
    [Obsolete ("Add is not safe for all caches (can lead to race conditions). Try using GetOrCreateValue() instead.", false)]
    void Add (TKey key, TValue value);
    TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory);

    bool TryGetValue (TKey key, out TValue value);

    void Clear();
  }
}

