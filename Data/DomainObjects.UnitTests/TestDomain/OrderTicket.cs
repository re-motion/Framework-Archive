using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class OrderTicket : TestDomainBase
  {
    public static OrderTicket GetObject (ObjectID id)
    {
      return (OrderTicket) DomainObject.GetObject (id);
    }

    // New OrderTickets need an associated order for correct initialization.
    public static OrderTicket Create ()
    {
      return DomainObject.Create<OrderTicket> ();
    }

    // New OrderTickets need an associated order for correct initialization.
    public static OrderTicket Create (Order order)
    {
      OrderTicket orderTicket = Create();
      orderTicket.Initialize (order);
      return orderTicket;
    }

    protected OrderTicket (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    protected virtual void Initialize (Order order) 
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string FileName { get; set; }

    [DBBidirectionalRelation ("OrderTicket", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
