using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
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
      "A scalar query 'OrderSumQuery' must not specify a collectionType.")]    
  public void ScalarQueryWithCollectionType ()
  {
    QueryConfigurationLoader loader = new QueryConfigurationLoader (@"scalarQueryWithCollectionType.xml", @"queries.xsd");
    loader.GetQueryDefinitions ();
  }

  [Test]
  public void InitializeWithFileNames ()
  {
    try
    {
      QueryConfiguration.SetCurrent (
          new QueryConfiguration (@"queriesForLoaderTest.xml", @"queries.xsd"));

      string configurationFile = Path.GetFullPath (@"queriesForLoaderTest.xml");
      string schemaFile = Path.GetFullPath (@"queries.xsd");

      Assert.AreEqual (configurationFile, QueryConfiguration.Current.ConfigurationFile);
      Assert.AreEqual (schemaFile, QueryConfiguration.Current.SchemaFile);
    }
    finally
    {
      QueryConfiguration.SetCurrent (null);
    }
  }

  [Test]
  public void ApplicationName ()
  {
    Assert.AreEqual ("UnitTests", QueryConfiguration.Current.ApplicationName);
  }
  
  [Test]
  public void NumericIndexer ()
  {
    Assert.IsNotNull (QueryConfiguration.Current[0]);
  }

  private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
  {
    QueryDefinitionCollection queries = new QueryDefinitionCollection ();

    queries.Add (QueryFactory.CreateOrderQueryDefinition ());
    queries.Add (QueryFactory.CreateCustomerTypeQueryDefinition ());
    queries.Add (QueryFactory.CreateOrderSumQueryDefinition ());

    return queries;
  }
}
}
