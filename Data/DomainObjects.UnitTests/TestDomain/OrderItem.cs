using System;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class OrderItem : TestDomainBase
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem> ().With();
    }

    public static OrderItem NewObject (Order order)
    {
      return NewObject<OrderItem> ().With (order);
    }

    public new static OrderItem GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderItem> (id);
    }

    public new static OrderItem GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return OrderItem.GetObject (id);
      }
    }

    protected OrderItem()
    {
    }

    protected OrderItem (Order order)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
