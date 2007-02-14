using System;

namespace Rubicon.Collections
{
  //TODO: Doc
  public interface ICache<TKey, TValue> : INullableObject
  {
    void Add (TKey key, TValue value);

    TValue GetOrCreateValue (TKey key, Func<TValue> valueFactory);

    bool TryGetValue (TKey key, out TValue value);

    void Clear();
  }
}