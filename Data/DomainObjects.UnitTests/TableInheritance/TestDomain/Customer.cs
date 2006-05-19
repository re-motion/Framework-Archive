using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
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
  }
}
