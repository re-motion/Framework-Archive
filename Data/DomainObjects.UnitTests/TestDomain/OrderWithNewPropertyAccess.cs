using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [FactoryInstantiated]
  public abstract class OrderWithNewPropertyAccess : TestDomainBase
  {
    // types

    // static members and constants

    public static new OrderWithNewPropertyAccess GetObject (ObjectID id)
    {
      return DomainObject.GetObject <OrderWithNewPropertyAccess> (id);
    }

    // member fields

    // construction and disposing

    public OrderWithNewPropertyAccess ()
    {
    }

    public OrderWithNewPropertyAccess (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected OrderWithNewPropertyAccess (DataContainer dataContainer)
      : base (dataContainer)
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

    public virtual DomainObject NotInMappingRelated
    {
      get { return GetRelatedObject (); }
    }
  }
}
