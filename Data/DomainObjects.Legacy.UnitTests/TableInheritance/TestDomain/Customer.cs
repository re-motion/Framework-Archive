using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public enum CustomerType
  {
    Standard = 0,
    Premium = 1
  }

  public class Customer : Person
  {
    // types

    // static members and constants

    public static new Customer GetObject (ObjectID id)
    {
      return (Customer) DomainObject.GetObject (id);
    }

    public static new Customer GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Customer) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Customer ()
    {
    }

    public Customer (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Customer (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public CustomerType CustomerType
    {
      get { return (CustomerType) DataContainer.GetValue ("CustomerType"); }
      set { DataContainer.SetValue ("CustomerType", value); }
    }

    public DateTime CustomerSince
    {
      get { return DataContainer.GetDateTime ("CustomerSince"); }
      set { DataContainer.SetValue ("CustomerSince", value); }
    }

    public Region Region
    {
      get { return (Region) GetRelatedObject ("Region"); }
      set { SetRelatedObject ("Region", value); }
    }

    public DomainObjectCollection Orders
    {
      get { return GetRelatedObjects ("Orders"); }
    }
  }
}
