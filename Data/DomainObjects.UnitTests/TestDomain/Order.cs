using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Order : TestDomainBase
  {
    public static Order NewObject ()
    {
      return NewObject<Order> ().With();
    }

    public static Order NewObject (ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return Order.NewObject();
      }
    }

    public new static Order GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Order> (id);
    }

    public new static Order GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<Order> (id);
      }
    }

    public new static Order GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<Order> (id, includeDeleted);
      }
    }

    protected Order ()
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
    public abstract ObjectList<OrderItem> OrderItems { get; }

    public new void PreparePropertyAccess (string propertyName)
    {
      base.PreparePropertyAccess (propertyName);
    }

    public new void PropertyAccessFinished()
    {
      base.PropertyAccessFinished();
    }

    [StorageClassNone]
    public new PropertyAccessor CurrentProperty
    {
      get { return base.CurrentProperty; }
    }
  }
}