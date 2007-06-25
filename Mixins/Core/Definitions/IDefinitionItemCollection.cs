using System.Collections.Generic;

namespace Rubicon.Mixins.Definitions
{
  public interface IDefinitionItemCollection<TKey, TValue> : IEnumerable<TValue>
  {
    TValue[] ToArray ();
    int Count { get; }
    bool HasItem (TKey key);
    TValue this[int index] { get; }
    TValue this [TKey key] { get; }
  }
}