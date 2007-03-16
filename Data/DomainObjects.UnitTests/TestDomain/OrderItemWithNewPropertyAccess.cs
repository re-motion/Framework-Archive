using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [FactoryInstantiated]
  public class OrderItemWithNewPropertyAccess : TestDomainBase
  {
    // types

    // static members and constants

    public static new OrderItemWithNewPropertyAccess GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderItemWithNewPropertyAccess> (id);
    }

    // member fields

    // construction and disposing

    public OrderItemWithNewPropertyAccess ()
    {
    }

    public OrderItemWithNewPropertyAccess (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected OrderItemWithNewPropertyAccess (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public OrderItemWithNewPropertyAccess (OrderWithNewPropertyAccess order)
    {
      ArgumentUtility.CheckNotNull ("order", order);

      this.Order = order;
    }

    // methods and properties

    public int Position
    {
      get { return (int) DataContainer["Position"]; }
      set { DataContainer["Position"] = value; }
    }

    public string Product
    {
      get { return (string) DataContainer["Product"]; }
      set { DataContainer["Product"] = value; }
    }

    public OrderWithNewPropertyAccess Order
    {
      get { return (OrderWithNewPropertyAccess) GetRelatedObject ("Order"); }
      set { SetRelatedObject ("Order", value); }
    }
  }
}
