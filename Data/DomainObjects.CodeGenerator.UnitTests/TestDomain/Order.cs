using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TestDomain
{
public class Order : BindableDomainObject
{
  // types

  // static members and constants

  public static new Order GetObject (ObjectID id)
  {
    return (Order) DomainObject.GetObject (id);
  }

  public static new Order GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (Order) DomainObject.GetObject (id, clientTransaction);
  }

  // member fields

  // construction and disposing

  public Order ()
  {
  }

  public Order (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Order (DataContainer dataContainer) : base (dataContainer)
  {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
  }

  // methods and properties

  public int Number
  {
    get { return (int) DataContainer["Number"]; }
    set { DataContainer["Number"] = value; }
  }

  public OrderPriority Priority
  {
    get { return (OrderPriority) DataContainer["Priority"]; }
    set { DataContainer["Priority"] = value; }
  }

  public Customer Customer
  {
    get { return (Customer) GetRelatedObject ("Customer"); }
    set { SetRelatedObject ("Customer", value); }
  }

  public Official Official
  {
    get { return (Official) GetRelatedObject ("Official"); }
    set { SetRelatedObject ("Official", value); }
  }

  public DomainObjectCollection OrderItems
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("OrderItems"); }
    set { } // marks property OrderItems as modifiable
  }

}
}
