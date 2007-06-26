using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
public class OrderItem : TestDomainBase
{
  // types

  // static members and constants

  public static new OrderItem GetObject (ObjectID id)
  {
    return DomainObject.GetObject<OrderItem> (id);
  }

  // member fields

  // construction and disposing

  public OrderItem ()
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
    get { return Properties["Position"].GetValue<int>(); }
    set { Properties["Position"].SetValue (value); }
  }

  public string Product
  {
    get { return Properties["Product"].GetValue<string>(); }
    set { Properties["Product"].SetValue (value); }
  }

  public Order Order
  {
    get { return Properties["Order"].GetValue<Order>(); }
    set { Properties["Order"].SetValue (value); }
  }
}
}
