using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public abstract class DefinitionItemCollectionBase<TKey, TValue> : IEnumerable<TValue>
      where TValue : IVisitableDefinition
  {
    private List<TValue> _orderedItems = new List<TValue> ();

    private KeyMaker _keyMaker;

    public delegate TKey KeyMaker (TValue value);

    public DefinitionItemCollectionBase (KeyMaker keyMaker)
    {
      ArgumentUtility.CheckNotNull ("keyMaker", keyMaker);
      _keyMaker = keyMaker;
    }

    public IEnumerator<TValue> GetEnumerator ()
    {
      return _orderedItems.GetEnumerator ();
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public int Count
    {
      get { return _orderedItems.Count; }
    }

    public abstract bool HasItem (TKey key);

    protected internal void Add (TValue newItem)
    {
      ArgumentUtility.CheckNotNull ("newItem", newItem);
      TKey key = _keyMaker (newItem);

      CustomizedAdd (key, newItem);

      _orderedItems.Add (newItem);
    }

    protected abstract void CustomizedAdd (TKey key, TValue value);

    public TValue this[int index]
    {
      get { return _orderedItems[index]; }
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
