using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Order : TestDomainBase
  {
    public new static Order GetObject (ObjectID id)
    {
      return (Order) DomainObject.GetObject (id);
    }

    public static Order Create ()
    {
      return DomainObject.Create<Order> ();
    }

    public static Order Create (ClientTransaction clientTransaction)
    {
      return DomainObject.Create<Order> (clientTransaction);
    }

    protected Order (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBColumn ("OrderNo")]
    public abstract int OrderNumber { get; set; }

    public abstract DateTime DeliveryDate { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Official Official { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public abstract OrderTicket OrderTicket { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public virtual ObjectList<OrderItem> OrderItems { get { return (ObjectList<OrderItem>) GetRelatedObjects(); } }
  }
}