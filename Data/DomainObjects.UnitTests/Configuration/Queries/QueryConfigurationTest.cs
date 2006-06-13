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
  public class QueryConfigurationTest : StandardMappingTest
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
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"queriesForLoaderTest.xml");
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
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"scalarQueryWithCollectionType.xml");
      loader.GetQueryDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException), 
        "Error while reading query configuration: The root element has namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidMappingNamespace'"
        + " but was expected to have 'http://www.rubicon-it.com/Data/DomainObjects/Queries/1.0'.")]
    public void QueryConfigurationWithInvalidNamespace ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"queriesWithInvalidNamespace.xml");
    }

    [Test]
    public void InitializeWithFileNames ()
    {
      try
      {
        QueryConfiguration.SetCurrent (
            new QueryConfiguration (@"queriesForLoaderTest.xml"));

        string configurationFile = Path.GetFullPath (@"queriesForLoaderTest.xml");

        Assert.AreEqual (configurationFile, QueryConfiguration.Current.ConfigurationFile);
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

    [Test]
    public void Contains ()
    {
      Assert.IsFalse (QueryConfiguration.Current.Contains (QueryFactory.CreateCustomerTypeQueryDefinition ()));
      Assert.IsTrue (QueryConfiguration.Current.Contains (QueryConfiguration.Current["OrderNoSumByCustomerNameQuery"]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNull ()
    {
      QueryConfiguration.Current.Contains (null);
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
