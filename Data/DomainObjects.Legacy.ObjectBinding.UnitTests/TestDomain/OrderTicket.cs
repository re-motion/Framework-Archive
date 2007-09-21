using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
public class OrderTicket : TestDomainBase
{
  // types

  // static members and constants

  public static new OrderTicket GetObject (ObjectID id)
  {
    return DomainObject.GetObject<OrderTicket> (id);
  }

  // member fields

  // construction and disposing

  // New OrderTickets need an associated order for correct initialization.
  public OrderTicket (Order order)
  {
    ArgumentUtility.CheckNotNull ("order", order);
    Order = order;
  }

  protected OrderTicket (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string FileName
  {
    get { return Properties["FileName"].GetValue<string>(); }
    set { Properties["FileName"].SetValue (value); }
  }

  public Order Order
  {
    get { return Properties["Order"].GetValue<Order>(); }
    set { Properties["Order"].SetValue (value); }
  }
}
}
