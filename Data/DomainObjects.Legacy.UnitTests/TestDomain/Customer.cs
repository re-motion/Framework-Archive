using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.NullableValueTypes;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Customer : Company
  {
    // types

    public enum CustomerType
    {
      Standard = 0,
      Premium = 1,
      Gold = 2
    }

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

    public NaDateTime CustomerSince
    {
      get { return (NaDateTime) DataContainer.GetValue ("CustomerSince"); }
      set { DataContainer.SetValue ("CustomerSince", value); }
    }

    public CustomerType Type
    {
      get { return (CustomerType) DataContainer["Type"]; }
      set { DataContainer.SetValue ("Type", value); }
    }

    public OrderCollection Orders
    {
      get { return (OrderCollection) GetRelatedObjects ("Orders"); }
    }
  }
}
