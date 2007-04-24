using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class DefinitionItemCollection<TKey, TValue> : DefinitionItemCollectionBase<TKey, TValue>
      where TValue : IVisitableDefinition
  {
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue> ();

    public DefinitionItemCollection (KeyMaker keyMaker, Predicate<TValue> guardian) : base (keyMaker, guardian)
    {
    }

    public DefinitionItemCollection (KeyMaker keyMaker) : base (keyMaker, null)
    {
    }

    public override bool HasItem (TKey key)
    {
      return _items.ContainsKey (key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      if (HasItem (key))
      {
        string message = string.Format ("Duplicate key {0} for item {1}.", key, value);
        throw new InvalidOperationException (message);
      }
      _items.Add (key, value);
    }

    public TValue this[TKey key]
    {
      get { return HasItem (ArgumentUtility.CheckNotNull("key", key)) ? _items[key] : default (TValue); }
    }
  }
}
