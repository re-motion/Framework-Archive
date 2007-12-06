using System;
using Rubicon.Data.DomainObjects.Infrastructure;

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
      return (Customer) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Customer ()
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
      get { return (DateTime) DataContainer.GetValue ("CustomerSince"); }
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
