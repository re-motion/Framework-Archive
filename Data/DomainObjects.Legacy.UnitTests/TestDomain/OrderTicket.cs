using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class OrderTicket : TestDomainBase
  {
    // types

    // static members and constants

    public static new OrderTicket GetObject (ObjectID id)
    {
      return (OrderTicket) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    // Default constructor for testing purposes.
    public OrderTicket ()
    {
    }

    // New OrderTickets need an associated order for correct initialization.
    public OrderTicket (Order order)
      : this (order, ClientTransaction.Current)
    {
    }

    // New OrderTickets need an associated order for correct initialization.
    public OrderTicket (Order order, ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    protected OrderTicket (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string FileName
    {
      get { return DataContainer.GetString ("FileName"); }
      set { DataContainer.SetValue ("FileName", value); }
    }

    public Order Order
    {
      get { return (Order) GetRelatedObject ("Order"); }
      set { SetRelatedObject ("Order", value); }
    }
  }
}
