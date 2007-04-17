using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Order : DomainObject
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

    protected Order (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public int Number
    {
      get { return (int) DataContainer.GetValue ("Number"); }
      set { DataContainer.SetValue ("Number", value); }
    }

    public DateTime OrderDate
    {
      get { return (DateTime) DataContainer.GetValue ("OrderDate"); }
      set { DataContainer.SetValue ("OrderDate", value); }
    }

    public Customer Customer
    {
      get { return (Customer) GetRelatedObject ("Customer"); }
      set { SetRelatedObject ("Customer", value); }
    }
  }
}
