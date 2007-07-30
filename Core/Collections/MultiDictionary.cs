using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Utilities;
using Rubicon.Reflection;

namespace Rubicon.Collections
{
  /// <summary>
  /// A dictionary that contains a <see cref="List{TValue}"/> of values for every key.
  /// </summary>
  [Serializable]
  public class MultiDictionary<TKey, TValue> : AutoInitDictionary<TKey, List<TValue>>
  {
    public MultiDictionary ()
    {
    }

    public MultiDictionary (IEqualityComparer<TKey> comparer)
      : base (comparer)
    {
    }

    /// <summary>
    /// Adds a value to the key's value list.
    /// </summary>
    public void Add (TKey key, TValue value)
    {
      this[key].Add (value);
    }
  }
}
