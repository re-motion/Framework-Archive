using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects
{
  [Serializable]
  public class ObjectList<T> : DomainObjectCollection, IList<T> where T : DomainObject
  {
    public ObjectList()
        : base (typeof (T))
    {
    }

    public ObjectList (ObjectList<T> collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    public int IndexOf (T item)
    {
      return base.IndexOf (item);
    }

    public void Insert (int index, T item)
    {
      base.Insert (index, item);
    }

    public new T this [int index]
    {
      get { return (T) base[index]; }
      set { base[index] = value; }
    }

    public new T this [ObjectID id]
    {
      get { return (T) base[id]; }
    }

    public new ObjectList<T> Clone ()
    {
      return (ObjectList<T>) base.Clone();
    }

    public void Add (T item)
    {
      base.Add (item);
    }

    bool ICollection<T>.Contains (T item)
    {
      return base.ContainsObject (item);
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      base.CopyTo (array, arrayIndex);
    }

    public bool Remove (T item)
    {
      bool result = ContainsObject (item);
      base.Remove (item);
      return result;
    }

    public IEnumerator<T> GetEnumerator ()
    {
      foreach (T t in (IEnumerable) this)
        yield return t;
    }
  }
}