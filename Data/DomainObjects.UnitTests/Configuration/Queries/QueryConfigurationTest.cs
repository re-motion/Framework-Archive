using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
  [TestFixture]
  public class QueryConfigurationTest : ReflectionBasedMappingTest
  {
    [Test]
    public void Loading ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml");
      QueryDefinitionCollection actualQueries = loader.GetQueryDefinitions ();
      QueryDefinitionCollection expectedQueries = CreateExpectedQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, actualQueries);
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException),
        ExpectedMessage = "A scalar query 'OrderSumQuery' must not specify a collectionType.")]
    public void ScalarQueryWithCollectionType ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"ScalarQueryWithCollectionType.xml");
      loader.GetQueryDefinitions ();
    }

    [Test]
    public void QueryConfigurationWithInvalidNamespace ()
    {
      string configurationFile = "QueriesWithInvalidNamespace.xml";
      try
      {
        QueryConfigurationLoader loader = new QueryConfigurationLoader (configurationFile);

        Assert.Fail ("QueryConfigurationException was expected");
      }
      catch (QueryConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading query configuration: The namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.rubicon-it.com/Data/DomainObjects/Queries/1.0'. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void InitializeWithFileNames ()
    {
      try
      {
        QueryConfiguration.SetCurrent (new QueryConfiguration (@"QueriesForLoaderTest.xml"));

        Assert.AreEqual (3, QueryConfiguration.Current.QueryDefinitions.Count);
        Assert.IsNotNull (QueryConfiguration.Current.QueryDefinitions["OrderQuery"]);
      }
      finally
      {
        QueryConfiguration.SetCurrent (null);
      }
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
