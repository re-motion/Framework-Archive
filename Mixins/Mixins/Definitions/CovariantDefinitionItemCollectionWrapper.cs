using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  [Serializable]
  public class CovariantDefinitionItemCollectionWrapper<TKey, TValue, TValueBase> : IDefinitionItemCollection<TKey, TValueBase>
      where TValue : class, TValueBase
      where TValueBase : IVisitableDefinition
  {
    private DefinitionItemCollection<TKey, TValue> _items;

    public CovariantDefinitionItemCollectionWrapper(DefinitionItemCollection<TKey, TValue> items)
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

    public bool HasItem (TKey key)
    {
      return _items.HasItem (key);
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
