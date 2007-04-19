using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class DefinitionItemCollection<TKey, TValue> : IEnumerable<TValue>
      where TValue : IVisitableDefinition
  {
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue> ();
    private KeyMaker _keyMaker;

    public delegate TKey KeyMaker (TValue value);

    public DefinitionItemCollection (KeyMaker keyMaker)
    {
      ArgumentUtility.CheckNotNull ("keyMaker", keyMaker);
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

    public int Count
    {
      get { return _items.Count; }
    }

    public bool HasItem (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items.ContainsKey (key);
    }

    internal void Add (TValue newItem)
    {
      ArgumentUtility.CheckNotNull ("newItem", newItem);
      TKey key = _keyMaker (newItem);
      if (HasItem (key))
      {
        string message = string.Format ("Duplicate key {0} for item {1}.", key, newItem);
        throw new InvalidOperationException (message);
      }
      _items.Add (key, newItem);
    }

    public TValue this[TKey key]
    {
      get { return HasItem (ArgumentUtility.CheckNotNull("key", key)) ? _items[key] : default (TValue); }
    }

    internal void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      foreach (TValue value in this)
      {
        value.Accept (visitor);
      }
    }
  }
}
