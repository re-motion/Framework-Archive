using System;

using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
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

  // member fields

  // construction and disposing

  public Customer ()
  {
  }

  protected Customer (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public NaDateTime CustomerSince
  {
    get { return DataContainer.GetNaDateTime ("CustomerSince"); }
    set { DataContainer["CustomerSince"] = value; }
  }

  public CustomerType Type
  {
    get { return (CustomerType) DataContainer["CustomerType"]; }
    set { DataContainer["CustomerType"] = value; }
  }

  public OrderCollection Orders 
  {
    get { return (OrderCollection) GetRelatedObjects ("Orders"); }
  }
}
}
