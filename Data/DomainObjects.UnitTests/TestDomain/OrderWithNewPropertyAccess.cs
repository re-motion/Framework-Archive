using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [FactoryInstantiated]
  [NotAbstract]
  public abstract class OrderWithNewPropertyAccess : TestDomainBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public OrderWithNewPropertyAccess (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    // methods and properties

    [AutomaticProperty]
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

    public virtual Customer OriginalCustomer
    {
      get { return (Customer) GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"); }
    }

    public virtual DomainObjectCollection OrderItems
    {
      get { return GetRelatedObjects (); }
    }

    public virtual int NotInMapping
    {
      get { return GetPropertyValue<int> (); }
    }

    public virtual OrderWithNewPropertyAccess NotInMappingRelated
    {
      get { return (OrderWithNewPropertyAccess) GetRelatedObject (); }
    }
  }
}
