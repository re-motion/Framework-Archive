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

    public static OrderItem NewObject ()
    {
      return DomainObject.NewObject<OrderItem> ().With();
    }

    public static OrderItem NewObject (Order order)
    {
      OrderItem orderItem = DomainObject.NewObject<OrderItem> ().With (order);
      return orderItem;
    }

    protected OrderItem()
    {
    }

    protected OrderItem (Order order)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    protected OrderItem (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
