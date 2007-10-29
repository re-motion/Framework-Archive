using System;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting.Configuration;
using Rubicon.Data.DomainObjects.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
  [TestFixture]
  public class QueryConfigurationTest : StandardMappingTest
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
    public void Deserialize_WithQueryFiles ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add name=""unique1"" filename=""..\..\myqueries1.xml""/>
                <add name=""unique2"" filename=""..\..\myqueries2.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (2));
      Assert.That (configuration.QueryFiles[0].Name, Is.EqualTo ("unique1"));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo (@"..\..\myqueries1.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries1.xml")));
      Assert.That (configuration.QueryFiles[1].Name, Is.EqualTo ("unique2"));
      Assert.That (configuration.QueryFiles[1].FileName, Is.EqualTo (@"..\..\myqueries2.xml"));
      Assert.That (configuration.QueryFiles[1].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries2.xml")));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "The entry 'unique' has already been added.")]
    public void Deserialize_WithNonUniqueNames ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add name=""unique"" filename=""..\..\myqueries1.xml""/>
                <add name=""unique"" filename=""..\..\myqueries2.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
      Assert.Fail ("Expected exception.");
    }

    [Test]
    public void QueryConfiguration_WithFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].Name);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "QueriesForLoaderTest.xml"), configuration.QueryFiles[0].RootedFileName);
    }

    [Test]
    public void QueryConfiguration_WithRootedFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration (@"c:\QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual (@"c:\QueriesForLoaderTest.xml", configuration.QueryFiles[0].Name);
      Assert.AreEqual (@"c:\QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
    }

    [Test]
    public void QueryConfiguration_WithMultipleFileNames ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml", "Query.xml");

      Assert.AreEqual (2, configuration.QueryFiles.Count);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].Name);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "QueriesForLoaderTest.xml"), configuration.QueryFiles[0].RootedFileName);
      Assert.AreEqual ("Query.xml", configuration.QueryFiles[1].Name);
      Assert.AreEqual ("Query.xml", configuration.QueryFiles[1].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Query.xml"), configuration.QueryFiles[1].RootedFileName);
    }

    [Test]
    public void GetDefinitions ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml");
      QueryDefinitionCollection expectedQueries = loader.GetQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_WithMultipleFiles ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml", "Queries.xml");

      QueryConfigurationLoader loader1 = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml");
      QueryConfigurationLoader loader2 = new QueryConfigurationLoader (@"Queries.xml");
      QueryDefinitionCollection expectedQueries = loader1.GetQueryDefinitions ();
      expectedQueries.Merge (loader2.GetQueryDefinitions());

      Assert.IsTrue (expectedQueries.Count > loader1.GetQueryDefinitions ().Count);

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_UsesRootedPath ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");
      string oldDirectory = Environment.CurrentDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.IsNotEmpty (configuration.QueryDefinitions);
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }

    [Test]
    public void CollectionTypeSupportsTypeUtilityNotation ()
    {
      QueryDefinitionCollection queries = DomainObjectsConfiguration.Current.Query.QueryDefinitions;
      Assert.AreSame (typeof (SpecificOrderCollection), queries["QueryWithSpecificCollectionType"].CollectionType);
    }

    private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection ();

      queries.Add (QueryFactory.CreateOrderQueryWithCustomCollectionType ());
      queries.Add (QueryFactory.CreateOrderQueryDefinitionWithObjectListOfOrder ());
      queries.Add (QueryFactory.CreateCustomerTypeQueryDefinition ());
      queries.Add (QueryFactory.CreateOrderSumQueryDefinition ());

      return queries;
    }
  }
}
