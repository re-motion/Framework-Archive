using System;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class QueryFactory
  {
    public static QueryDefinition CreateOrderQueryDefinition()
    {
      return new QueryDefinition (
          "OrderQuery",
          "TestDomain",
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.Collection,
          typeof (OrderCollection));
    }

    public static QueryDefinition CreateCustomerTypeQueryDefinition()
    {
      return new QueryDefinition (
          "CustomerTypeQuery",
          "TestDomain",
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.Collection,
          typeof (DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryDefinition()
    {
      return new QueryDefinition (
          "OrderSumQuery",
          "TestDomain",
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.Scalar);
    }
  }
}