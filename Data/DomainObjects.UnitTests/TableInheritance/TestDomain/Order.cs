using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Order")]
  [DBTable ("TableInheritance_Order")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class Order: DomainObject
  {
    public new static Order GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Order) DomainObject.GetObject (id, clientTransaction);
    }

    public static Order NewObject()
    {
      return NewObject<Order>().With();
    }

    protected Order()
    {
    }

    protected Order (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract int Number { get; set; }

    public abstract DateTime OrderDate { get; set; }

    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }
  }
}