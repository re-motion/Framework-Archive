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
          "select [OrderView].* from [OrderView] inner join [CustomerView] where [CustomerView].[ID] = @customerID order by [Order_OrderNo] asc;",
          QueryType.Collection,
          typeof (OrderCollection));
    }

    public static QueryDefinition CreateCustomerTypeQueryDefinition()
    {
      return new QueryDefinition (
          "CustomerTypeQuery",
          "TestDomain",
          "select [CustomerView].* from [CustomerView] where [Customer_CustomerType] = @customerType order by [Company_Name] asc;",
          QueryType.Collection,
          typeof (DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryDefinition()
    {
      return new QueryDefinition (
          "OrderSumQuery",
          "TestDomain",
          "select sum(quantity) from [OrderView] where [Order_CustomerID] = @customerID;",
          QueryType.Scalar);
    }
  }
}