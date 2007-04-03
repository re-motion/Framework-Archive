using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
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

    public Customer (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Customer (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected Customer (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public NaDateTime CustomerSince
    {
      get { return DataContainer.GetNaDateTime ("CustomerSince"); }
      set { DataContainer.SetValue ("CustomerSince", value); }
    }

    [DBColumn ("CustomerType")]
    public CustomerType Type
    {
      get { return (CustomerType) DataContainer["Type"]; }
      set { DataContainer.SetValue ("Type", value); }
    }

    [DBBidirectionalRelation ("Customer", SortExpression = "OrderNo asc")]
    public OrderCollection Orders
    {
      get { return (OrderCollection) GetRelatedObjects ("Orders"); }
    }
  }
}
