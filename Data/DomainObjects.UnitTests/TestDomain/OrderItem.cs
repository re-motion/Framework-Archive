using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
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

  protected OrderItem (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public int Position 
  {
    get { return DataContainer.GetInt32 ("Position"); }
    set { DataContainer["Position"] = value; }
  }

  public string Product
  {
    get { return DataContainer.GetString ("Product"); }
    set { DataContainer["Product"] = value; }
  }

  public Order Order
  {
    get { return (Order) GetRelatedObject ("Order"); }
    set { SetRelatedObject ("Order", value); }
  }
}
}
