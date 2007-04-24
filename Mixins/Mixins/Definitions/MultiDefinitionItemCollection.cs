using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class MultiDefinitionItemCollection<TKey, TValue> : DefinitionItemCollectionBase<TKey, TValue>
      where TValue : IVisitableDefinition
  {
    private MultiDictionary<TKey, TValue> _items = new MultiDictionary<TKey, TValue>();

    public MultiDefinitionItemCollection (KeyMaker keyMaker)
        : base (keyMaker)
    {
    }

    public override bool HasItem (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items.ContainsKey (key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _items.Add (key, value);
    }

    public IEnumerable<TValue> this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        return _items[key];
      }
    }

    public int GetItemCount (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items.GetValueCount (key);
    }

    public TValue GetFirstItem (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (GetItemCount (key) == 0)
        throw new ArgumentException ("There is no item with the given key.", "key");
      else
        return _items.GetFirstValue (key);
    }
  }

  // TODO: replace with Rubicon.Core.Collections.MultiDictionary when available
  [Serializable]
  internal class MultiDictionary<TKey, TValue>
  {
    private Dictionary<TKey, List<TValue>> _innerDictionary = new Dictionary<TKey, List<TValue>> ();

    public void Add (TKey key, TValue value)
    {
      GetOrCreateValueList (key).Add (value);
    }

    private List<TValue> GetOrCreateValueList (TKey key)
    {
      if (!_innerDictionary.ContainsKey (key))
        _innerDictionary.Add (key, new List<TValue> ());
      return _innerDictionary[key];
    }

    public bool ContainsKey (TKey key)
    {
      return _innerDictionary.ContainsKey (key);
    }

    public int GetValueCount (TKey key)
    {
      if (_innerDictionary.ContainsKey (key))
        return _innerDictionary[key].Count;
      else
        return 0;
    }

    public IEnumerable<TValue> this[TKey key]
    {
      get { return _innerDictionary[key]; }
    }

    public TValue GetFirstValue (TKey key)
    {
      return _innerDictionary[key][0];
    }
  }
}
