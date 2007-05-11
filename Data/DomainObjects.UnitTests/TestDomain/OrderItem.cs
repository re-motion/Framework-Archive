using System;
using Rubicon.Utilities;

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
