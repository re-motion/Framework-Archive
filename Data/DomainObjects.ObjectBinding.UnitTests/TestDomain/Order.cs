using System;

using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
public class Order : TestDomainBase
{
  // types

  // static members and constants

  public static new Order GetObject (ObjectID id)
  {
    return (Order) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public Order ()
  {
  }

  public Order (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Order (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public int OrderNumber
  {
    get { return (int) DataContainer["OrderNumber"]; }
    set { DataContainer["OrderNumber"] = value; }
  }

  public DateTime DeliveryDate
  {
    get { return (DateTime) DataContainer["DeliveryDate"]; }
    set { DataContainer["DeliveryDate"] = value; }
  }

  public OrderTicket OrderTicket
  {
    get { return (OrderTicket) GetRelatedObject ("OrderTicket"); }
    set { SetRelatedObject ("OrderTicket", value); }
  }

  public DomainObjectCollection OrderItems
  {
    get { return GetRelatedObjects ("OrderItems"); }
  }
}
}
