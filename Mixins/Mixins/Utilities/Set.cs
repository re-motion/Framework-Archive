using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mixins.Utilities
{
  [DebuggerDisplay ("Count = {_items.Count}")]
  [Serializable]
  internal class Set<T> : IEnumerable<T>
  {
    private Dictionary<T, T> _items = new Dictionary<T, T>();

    public Set()
    {
    }

    public Set(IEnumerable<T> initialItems)
    {
      AddRange (initialItems);
    }

    public void AddRange (IEnumerable<T> items)
    {
      foreach (T item in items)
        Add (item);
    }

    public void Add (T item)
    {
      if (ShouldAddItem (item) && !Contains (item))
        _items.Add (item, item);
    }

    protected virtual bool ShouldAddItem (T item)
    {
      return true;
    }

    public bool Contains (T item)
    {
      return _items.ContainsKey (item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _items.Keys.GetEnumerator();
    }
  }
}