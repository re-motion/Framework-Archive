using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Order")]
  [DBTable (Name = "TableInheritance_Order")]
  [NotAbstract]
  public abstract class Order: DomainObject
  {
    public new static Order GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Order) DomainObject.GetObject (id, clientTransaction);
    }

    public static Order Create()
    {
      return Create<Order>();
    }

    protected Order (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    public abstract int Number { get; set; }

    public abstract DateTime OrderDate { get; set; }

    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }
  }
}