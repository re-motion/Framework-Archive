using System;

using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
public class OrderItem : TestDomainBase
{
  // types

  // static members and constants

  public static new OrderItem GetObject (ObjectID id)
  {
    return (OrderItem) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public OrderItem ()
  {
  }

  public OrderItem (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected OrderItem (DataContainer dataContainer) : base (dataContainer)
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
