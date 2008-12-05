// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using File=System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryConfigurationTest : StandardMappingTest
  {
    [Test]
    public void Loading ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForLoaderTest.xml");
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
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\ScalarQueryWithCollectionType.xml");
      loader.GetQueryDefinitions ();
    }

    [Test]
    public void QueryConfigurationWithInvalidNamespace ()
    {
      string configurationFile = "DomainObjects\\Core\\QueriesWithInvalidNamespace.xml";
      try
      {
        QueryConfigurationLoader loader = new QueryConfigurationLoader (configurationFile);

        Assert.Fail ("QueryConfigurationException was expected");
      }
      catch (QueryConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading query configuration: The namespace 'http://www.re-motion.org/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.re-motion.org/Data/DomainObjects/Queries/1.0'. File: '{0}'.",
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
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries2.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (2));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo (@"..\..\myqueries1.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries1.xml")));
      Assert.That (configuration.QueryFiles[1].FileName, Is.EqualTo (@"..\..\myqueries2.xml"));
      Assert.That (configuration.QueryFiles[1].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries2.xml")));
    }

    [Test]
    public void GetDefaultQueryFilePath_BaseDirectory ()
    {
      QueryConfiguration configuration = new QueryConfiguration ();

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (0));
      Assert.That (configuration.QueryDefinitions.Count, Is.GreaterThan (0));

      Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));

      QueryConfigurationLoader loader = new QueryConfigurationLoader (configuration.GetDefaultQueryFilePath ());
      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (loader.GetQueryDefinitions (), configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefaultQueryFilePath_WithRelativeSearchPath ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = Path.GetPathRoot (AppDomain.CurrentDomain.BaseDirectory);
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory).Substring (setup.ApplicationBase.Length); // make a relative path

      new AppDomainRunner (setup, delegate (object[] args)
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (!File.Exists (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine ((string) args[0], "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run();
    }

    [Test]
    public void GetDefaultQueryFilePath_WithMultipleRelativeSearchPaths ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = Path.GetPathRoot (AppDomain.CurrentDomain.BaseDirectory);
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = @"A;B;C;Foo;" + Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory).Substring (setup.ApplicationBase.Length);  // make a relative path

      new AppDomainRunner (setup, delegate (object[] args)
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (!File.Exists (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine ((string) args[0], "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "No default query file found. Searched for one of the following files:\nC:\\queries.xml")]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists ()
    {
      AppDomainRunner.Run (@"C:\", delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      });
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "No default query file found. Searched for one of the following files:\nC:\\queries.xml\nC:\\Bin\\queries.xml\nC:\\Foo\\queries.xml")]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists_WithMultipleRelativeSearchPaths ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"C:\";
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = @"Bin;Foo";

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      }).Run ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"Two default query configuration files found", 
        MatchType = MessageMatch.Contains)]
    public void GetDefaultQueryFilePath_ThrowsIfMultipleQueryFilesExist ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = ".";  // simulate multiple files by searching the same directory twice

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
    }

    [Test]
    public void GetDefaultQueryFilePath_WithEmptyRelativeSearchPath ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = "";

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
    }

    [Test]
    public void Deserialize_WithNonUniqueNames ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries1.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      // unfortunately, this silently works because identical elements are not considered duplicates
    }

    [Test]
    public void QueryConfiguration_WithFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "QueriesForLoaderTest.xml"), configuration.QueryFiles[0].RootedFileName);
    }

    [Test]
    public void QueryConfiguration_WithRootedFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration (@"c:\QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual (@"c:\QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
    }

    [Test]
    public void QueryConfiguration_WithMultipleFileNames ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Q1.xml", "Q2.xml");

      Assert.AreEqual (2, configuration.QueryFiles.Count);
      Assert.AreEqual ("Q1.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Q1.xml"), configuration.QueryFiles[0].RootedFileName);
      Assert.AreEqual ("Q2.xml", configuration.QueryFiles[1].FileName);
      Assert.AreEqual (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Q2.xml"), configuration.QueryFiles[1].RootedFileName);
    }

    [Test]
    public void GetDefinitions ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("DomainObjects\\Core\\QueriesForLoaderTest.xml");

      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForLoaderTest.xml");
      QueryDefinitionCollection expectedQueries = loader.GetQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_WithMultipleFiles ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("DomainObjects\\Core\\QueriesForLoaderTest.xml", "DomainObjects\\Core\\QueriesForLoaderTest2.xml");

      QueryConfigurationLoader loader1 = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForLoaderTest.xml");
      QueryConfigurationLoader loader2 = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForLoaderTest2.xml");
      QueryDefinitionCollection expectedQueries = loader1.GetQueryDefinitions ();
      expectedQueries.Merge (loader2.GetQueryDefinitions());

      Assert.IsTrue (expectedQueries.Count > loader1.GetQueryDefinitions ().Count);

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void RootedPath_UnaffectedByDirectoryChange()
    {
      QueryConfiguration configuration = new QueryConfiguration ("DomainObjects\\Core\\QueriesForLoaderTest.xml");
      string pathBefore = configuration.QueryFiles[0].RootedFileName;

      string oldDirectory = AppDomain.CurrentDomain.BaseDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (pathBefore));
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }

    [Test]
    public void GetDefinitions_UsesRootedPath ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("DomainObjects\\Core\\QueriesForLoaderTest.xml");
      string oldDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
    public void CollectionType_SupportsTypeUtilityNotation ()
    {
      QueryDefinitionCollection queries = new QueryConfiguration ("DomainObjects\\QueriesForStandardMapping.xml").QueryDefinitions;
      Assert.AreSame (typeof (SpecificOrderCollection), queries["QueryWithSpecificCollectionType"].CollectionType);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"File '.*QueriesForLoaderTestDuplicate.xml' defines a duplicate "
        + @"for query definition 'OrderQueryWithCustomCollectionType'.", MatchType = MessageMatch.Regex)]
    public void DifferentQueryFiles_SpecifyingDuplicates ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("DomainObjects\\Core\\QueriesForLoaderTest.xml", "DomainObjects\\Core\\QueriesForLoaderTestDuplicate.xml");

      Dev.Null = configuration.QueryDefinitions;
    }

    private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection ();

      queries.Add (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ());
      queries.Add (TestQueryFactory.CreateOrderQueryDefinitionWithObjectListOfOrder ());
      queries.Add (TestQueryFactory.CreateCustomerTypeQueryDefinition ());
      queries.Add (TestQueryFactory.CreateOrderSumQueryDefinition ());

      return queries;
    }

    [Test]
    public void Load_ProviderFromDefaultStorageProvider ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForStorageGroupTest.xml");
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromDefaultStorageProvider"].StorageProviderID,
          Is.EqualTo (DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name));
    }

    [Test]
    public void Load_ProviderFromCustomStorageGroup ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForStorageGroupTest.xml");
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromCustomStorageGroup"].StorageProviderID,
          Is.EqualTo (DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions["TestDomain"].Name));
      Assert.That (
         queries["QueryFromCustomStorageGroup"].StorageProviderID,
         Is.Not.EqualTo (DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name));
    }

    [Test]
    public void Load_ProviderFromUndefinedStorageGroup ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"DomainObjects\\Core\\QueriesForStorageGroupTest.xml");
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromUndefinedStorageGroup"].StorageProviderID,
          Is.EqualTo (DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name));
    }
  }
}
