using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class OrderItem : TestDomainBase
  {
    // types

    // static members and constants

    public static OrderItem GetObject (ObjectID id)
    {
      return (OrderItem) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public OrderItem ()
    {
    }

    protected OrderItem (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public OrderItem (Order order)
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

    public Order Order
    {
      get { return (Order) GetRelatedObject ("Order"); }
      set { SetRelatedObject ("Order", value); }
    }
  }
}
