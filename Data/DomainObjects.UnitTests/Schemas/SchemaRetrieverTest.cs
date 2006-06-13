using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Xml;
using System.Xml;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.Schemas;

namespace Rubicon.Data.DomainObjects.UnitTests.Schemas
{
  [TestFixture]
  public class SchemaRetrieverTest
  {
    // types

    // static members and constants

    // member fields

    private SchemaRetriever _schemaRetrieverWithMapping;
    private SchemaRetriever _schemaRetrieverWithQueries;
    private SchemaRetriever _schemaRetrieverWithStorageProviders;

    // construction and disposing

    public SchemaRetrieverTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _schemaRetrieverWithMapping = new SchemaRetriever (SchemaRetriever.SchemaType.Mapping);
      _schemaRetrieverWithQueries = new SchemaRetriever (SchemaRetriever.SchemaType.Queries);
      _schemaRetrieverWithStorageProviders = new SchemaRetriever (SchemaRetriever.SchemaType.StorageProviders);
    }

    [Test]
    public void InitializeWithMapping ()
    {
      Assert.AreEqual (SchemaRetriever.SchemaType.Mapping, _schemaRetrieverWithMapping.Type);
      Assert.AreEqual (PrefixNamespace.MappingNamespace.Uri, _schemaRetrieverWithMapping.SchemaUri);
    }

    [Test]
    public void InitializeWithQueries ()
    {
      Assert.AreEqual (SchemaRetriever.SchemaType.Queries, _schemaRetrieverWithQueries.Type);
      Assert.AreEqual (PrefixNamespace.QueryConfigurationNamespace.Uri, _schemaRetrieverWithQueries.SchemaUri);
    }

    [Test]
    public void InitializeWithStorageProviders ()
    {
      Assert.AreEqual (SchemaRetriever.SchemaType.StorageProviders, _schemaRetrieverWithStorageProviders.Type);
      Assert.AreEqual (PrefixNamespace.StorageProviderConfigurationNamespace.Uri, _schemaRetrieverWithStorageProviders.SchemaUri);
    }

    [Test]
    public void IsDerivedFromSchemaBase ()
    {
      Assert.IsNotNull (_schemaRetrieverWithMapping as SchemaBase);
    }

    [Test]
    public void GetSchemaSetWithMapping ()
    {
      XmlSchemaSet schemaSet = _schemaRetrieverWithMapping.GetSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.MappingNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void GetSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = _schemaRetrieverWithQueries.GetSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.QueryConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void GetSchemaSetWithStorageProviders ()
    {
      XmlSchemaSet schemaSet = _schemaRetrieverWithStorageProviders.GetSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.StorageProviderConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }
  }
}
