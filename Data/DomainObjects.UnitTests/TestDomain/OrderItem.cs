using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class OrderItem : TestDomainBase
  {
    public static OrderItem GetObject (ObjectID id)
    {
      return (OrderItem) DomainObject.GetObject (id);
    }

    public static OrderItem Create ()
    {
      return DomainObject.Create<OrderItem> ();
    }

    public static OrderItem Create (Order order)
    {
      OrderItem orderItem = Create ();
      orderItem.Initialize (order);
      return orderItem;
    }

    protected OrderItem (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    protected virtual void Initialize (Order order)
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
