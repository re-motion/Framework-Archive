using System;

namespace Rubicon.Data.DomainObjects
{
  [Serializable]
  public class ObjectList<T> : DomainObjectCollection where T : DomainObject
  {
    public ObjectList()
        : base (typeof (T))
    {
    }

    public ObjectList (ObjectList<T> collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
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
  }
}