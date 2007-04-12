using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  [DBTable]
  [TestDomain]
  public abstract class OrderWithNewPropertyAccess : DomainObject
  {
    public static OrderWithNewPropertyAccess Create ()
    {
      return DomainObject.Create<OrderWithNewPropertyAccess>();
    }

    protected OrderWithNewPropertyAccess (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    [DBColumn( ("OrderNo"))]
    public abstract int OrderNumber { get; set; }

    public virtual DateTime DeliveryDate
    {
      get
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        return GetPropertyValue<DateTime> ();
      }
      set
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        SetPropertyValue (value);
      }
    }

    public virtual Customer Customer
    {
      get { return (Customer) GetRelatedObject (); }
      set { SetRelatedObject (value); }
    }

    [StorageClassNone]
    public virtual Customer OriginalCustomer
    {
      get { return (Customer) GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"); }
    }

    [DBBidirectionalRelation ("Order")]
    public virtual ObjectList<OrderItemWithNewPropertyAccess> OrderItems
    {
      get { return (ObjectList<OrderItemWithNewPropertyAccess>) GetRelatedObjects (); }
    }

    [StorageClassNone]
    public virtual int NotInMapping
    {
      get { return GetPropertyValue<int> (); }
    }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess NotInMappingRelated
    {
      get { return (OrderWithNewPropertyAccess) GetRelatedObject (); }
    }
  }
}
