using System;

using Rubicon.Data.DomainObjects.Configuration.Queries;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
public sealed class QueryFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  private QueryFactory ()
  {
  }

  // methods and properties

  public static QueryDefinition CreateOrderQueryDefinition ()
  {
    return new QueryDefinition (
        "OrderQuery", 
        "TestDomain", 
        "select Order.* from Order inner join Customer where Customer.ID = @customerID order by OrderNo asc;", 
        QueryType.Collection, 
        typeof (OrderCollection));
  }

  public static QueryDefinition CreateCustomerTypeQueryDefinition ()
  {
    return new QueryDefinition (
        "CustomerTypeQuery", 
        "TestDomain", 
        "select Customer.* from Customer where CustomerType = @customerType order by Name asc;", 
        QueryType.Collection, 
        typeof (DomainObjectCollection));
  }

  public static QueryDefinition CreateOrderSumQueryDefinition ()
  {
    return new QueryDefinition (
        "OrderSumQuery", 
        "TestDomain", 
        "select sum(quantity) from Order where CustomerID = @customerID;", 
        QueryType.Value);
  }
}
}
