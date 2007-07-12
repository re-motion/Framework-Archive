using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("Count = {Count}")]
  public class CovariantDefinitionItemCollectionWrapper<TKey, TValue, TValueBase> : IDefinitionItemCollection<TKey, TValueBase>
      where TValue : class, TValueBase
      where TValueBase : IVisitableDefinition
  {
    private UniqueDefinitionCollection<TKey, TValue> _items;

    public CovariantDefinitionItemCollectionWrapper(UniqueDefinitionCollection<TKey, TValue> items)
    {
      _items = items;
    }

    public TValueBase[] ToArray()
    {
      return _items.ToArray ();
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public bool ContainsKey (TKey key)
    {
      return _items.ContainsKey (key);
    }

    public TValueBase this [int index]
    {
      get { return _items[index]; }
    }

    public TValueBase this [TKey key]
    {
      get { return _items[key]; }
    }

    public IEnumerator<TValueBase> GetEnumerator()
    {
      foreach (TValue item in _items)
        yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
