using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  public class Order: TestDomainBase
  {
    // types

    // static members and constants

    public new static Order GetObject (ObjectID id)
    {
      return (Order) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Order()
    {
    }

    public Order (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected Order (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public Order (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    // methods and properties

    [DBColumn ("OrderNo")]
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

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public Official Official
    {
      get { return (Official) GetRelatedObject ("Official"); }
      set { SetRelatedObject ("Official", value); }
    }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public OrderTicket OrderTicket
    {
      get { return (OrderTicket) GetRelatedObject ("OrderTicket"); }
      set { SetRelatedObject ("OrderTicket", value); }
    }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public Customer Customer
    {
      get { return (Customer) GetRelatedObject ("Customer"); }
      set { SetRelatedObject ("Customer", value); }
    }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public ObjectList<OrderItem> OrderItems
    {
      get { return (ObjectList<OrderItem>) GetRelatedObjects ("OrderItems"); }
    }
  }
}