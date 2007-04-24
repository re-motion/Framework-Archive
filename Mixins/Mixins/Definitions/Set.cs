using System;
using System.Collections;
using System.Collections.Generic;

namespace Mixins.Definitions
{
  public class Set<T> : IEnumerable<T>
  {
    private Dictionary<T, T> _items = new Dictionary<T, T>();

    public virtual void Add (T item)
    {
      if (!Contains (item))
        _items.Add (item, item);
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
