using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderWithNewPropertyAccess: DomainObject
  {
    public static OrderWithNewPropertyAccess NewObject()
    {
      return NewObject<OrderWithNewPropertyAccess>().With();
    }

    public new static OrderWithNewPropertyAccess GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderWithNewPropertyAccess> (id);
    }

    protected OrderWithNewPropertyAccess()
    {
    }

    [DBColumn (("OrderNo"))]
    public abstract int OrderNumber { get; set; }

    public virtual DateTime DeliveryDate
    {
      get
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        return CurrentProperty<DateTime>().GetValue();
      }
      set
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        CurrentProperty<DateTime>().SetValue(value);
      }
    }

    public virtual Customer Customer
    {
      get { return CurrentProperty<Customer> ().GetValue (); }
      set { CurrentProperty<Customer> ().SetValue (value); }
    }

    [StorageClassNone]
    public virtual Customer OriginalCustomer
    {
      get { return (Customer) GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"); }
    }

    [DBBidirectionalRelation ("Order")]
    public virtual ObjectList<OrderItemWithNewPropertyAccess> OrderItems
    {
      get { return CurrentProperty<ObjectList<OrderItemWithNewPropertyAccess>>().GetValue(); }
    }

    [StorageClassNone]
    public virtual int NotInMapping
    {
      get { return CurrentProperty<int> ().GetValue (); }
      set { CurrentProperty<int> ().SetValue (value); }
    }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess NotInMappingRelated
    {
      get { return CurrentProperty<OrderWithNewPropertyAccess> ().GetValue (); }
      set { CurrentProperty<OrderWithNewPropertyAccess> ().SetValue (value); }
    }

    [StorageClassNone]
    public virtual ObjectList<OrderItem> NotInMappingRelatedObjects
    {
      get { return CurrentProperty<ObjectList<OrderItem>> ().GetValue (); }
    }
  }
}