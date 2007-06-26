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
    get { return Properties["OrderNumber"].GetValue<int>(); }
    set { Properties["OrderNumber"].SetValue (value); }
  }

  public DateTime DeliveryDate
  {
    get { return Properties["DeliveryDate"].GetValue<DateTime>(); }
    set { Properties["DeliveryDate"].SetValue (value); }
  }

  public OrderTicket OrderTicket
  {
    get { return Properties["OrderTicket"].GetValue<OrderTicket>(); }
    set { Properties["OrderTicket"].SetValue (value); }
  }

  [IsReadOnly]
  public ObjectList<OrderItem> OrderItems
  {
    get { return Properties["OrderItems"].GetValue <ObjectList<OrderItem>>(); }
  }
}
}
