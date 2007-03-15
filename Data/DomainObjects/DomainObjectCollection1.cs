using System;

namespace Rubicon.Data.DomainObjects
{
  public class DomainObjectCollection<T>: DomainObjectCollection where T: DomainObject
  {
    public DomainObjectCollection()
        : base (typeof (T))
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
  }
}