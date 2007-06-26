using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
[Serializable]
public class Order : TestDomainBase
{
  // types

  // static members and constants

  public static new Order GetObject (ObjectID id)
  {
    return DomainObject.GetObject<Order> (id);
  }

  // member fields

  // construction and disposing

  public Order ()
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

  [IsReadOnly]
  public ObjectList<OrderItem> OrderItems
  {
    get { return (ObjectList<OrderItem>) GetRelatedObjects ("OrderItems"); }
  }
}
}
