using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mixins.Definitions
{
  public class DefinitionItemCollection<TKey, TValue> : IEnumerable<TValue>
      where TValue : IVisitableDefinition
  {
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue> ();
    private KeyMaker _keyMaker;

    public delegate TKey KeyMaker (TValue value);

    public DefinitionItemCollection (KeyMaker keyMaker)
    {
      _keyMaker = keyMaker;
    }

    public IEnumerator<TValue> GetEnumerator ()
    {
      return _items.Values.GetEnumerator ();
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public bool HasItem (TKey key)
    {
      return _items.ContainsKey (key);
    }

    internal void Add (TValue newItem)
    {
      TKey key = _keyMaker (newItem);
      if (HasItem (key))
      {
        string message = string.Format ("Duplicate key {0} for item {1}.", key, newItem);
        throw new InvalidOperationException (message);
      }
      _items.Add (key, newItem);
    }

    public TValue Get (TKey key)
    {
      return HasItem (key) ? _items[key] : default (TValue);
    }

    internal void Accept (IDefinitionVisitor visitor)
    {
      foreach (TValue value in this)
      {
        value.Accept (visitor);
      }
    }
  }
}
