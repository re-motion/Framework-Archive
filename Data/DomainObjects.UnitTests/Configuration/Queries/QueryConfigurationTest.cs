using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Data.DomainObjects.Configuration.Queries;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
[TestFixture]
public class QueryConfigurationTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryConfigurationTest ()
  {
  }

  // methods and properties

  [Test]
  public void Loading ()
  {
    QueryConfigurationLoader loader = new QueryConfigurationLoader (@"queriesForLoaderTest.xml", @"queries.xsd");
    QueryDefinitionCollection actualQueries = loader.GetQueryDefinitions ();
    QueryDefinitionCollection expectedQueries = CreateExpectedQueryDefinitions ();

    QueryDefinitionChecker checker = new QueryDefinitionChecker ();
    checker.Check (expectedQueries, actualQueries);
  }

  [Test]
  [ExpectedException (typeof (QueryConfigurationException),
      "Value query 'OrderSumQuery' must not specify a collectionType.")]    
  public void ValueQueryWithCollectionType ()
  {
    QueryConfigurationLoader loader = new QueryConfigurationLoader (@"valueQueryWithCollectionType.xml", @"queries.xsd");
    loader.GetQueryDefinitions ();
  }
  
  private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
  {
    QueryDefinitionCollection queries = new QueryDefinitionCollection ();

    queries.Add (CreateOrderQueryDefinition ());
    queries.Add (CreateCustomerTypeQueryDefinition ());
    queries.Add (CreateOrderSumQueryDefinition ());

    return queries;
  }

  private QueryDefinition CreateOrderQueryDefinition ()
  {
    return new QueryDefinition (
        "OrderQuery", 
        "TestDomain", 
        "select Order.* from Order inner join Customer where Customer.ID = @customerID order by OrderNo asc;", 
        QueryType.Collection, 
        typeof (OrderCollection));
  }

  private QueryDefinition CreateCustomerTypeQueryDefinition ()
  {
    return new QueryDefinition (
        "CustomerTypeQuery", 
        "TestDomain", 
        "select Customer.* from Customer where CustomerType = @customerType order by Name asc;", 
        QueryType.Collection, 
        typeof (DomainObjectCollection));
  }

  private QueryDefinition CreateOrderSumQueryDefinition ()
  {
    return new QueryDefinition (
        "OrderSumQuery", 
        "TestDomain", 
        "select sum(quantity) from Order where CustomerID = @customerID;", 
        QueryType.Value);
  }
}
}
