using System;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [SecondStorageGroupAttribute]
  [Instantiable]
  public abstract class Official : DomainObject
  {
    public new static Official GetObject (ObjectID id)
    {
      return (Official) DomainObject.GetObject (id);
    }

    public static Official NewObject()
    {
      return NewObject<Official>().With();
    }

    protected Official()
    {
    }

    protected Official (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract OrderPriority ResponsibleForOrderPriority { get; set; }

    public abstract CustomerType ResponsibleForCustomerType { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}