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
  public class SchemaLoaderTest
  {
    // types

    // static members and constants

    // member fields

    private SchemaLoader _schemaLoaderWithMapping;
    private SchemaLoader _schemaLoaderWithQueries;
    private SchemaLoader _schemaLoaderWithStorageProviders;

    // construction and disposing

    public SchemaLoaderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _schemaLoaderWithMapping = new SchemaLoader (SchemaType.Mapping);
      _schemaLoaderWithQueries = new SchemaLoader (SchemaType.Queries);
      _schemaLoaderWithStorageProviders = new SchemaLoader (SchemaType.StorageProviders);
    }

    [Test]
    public void InitializeWithMapping ()
    {
      Assert.AreEqual (SchemaType.Mapping, _schemaLoaderWithMapping.Type);
      Assert.AreEqual (PrefixNamespace.MappingNamespace.Uri, _schemaLoaderWithMapping.SchemaUri);
    }

    [Test]
    public void InitializeWithQueries ()
    {
      Assert.AreEqual (SchemaType.Queries, _schemaLoaderWithQueries.Type);
      Assert.AreEqual (PrefixNamespace.QueryConfigurationNamespace.Uri, _schemaLoaderWithQueries.SchemaUri);
    }

    [Test]
    public void InitializeWithStorageProviders ()
    {
      Assert.AreEqual (SchemaType.StorageProviders, _schemaLoaderWithStorageProviders.Type);
      Assert.AreEqual (PrefixNamespace.StorageProviderConfigurationNamespace.Uri, _schemaLoaderWithStorageProviders.SchemaUri);
    }

    [Test]
    public void IsDerivedFromSchemaBase ()
    {
      Assert.IsNotNull (_schemaLoaderWithMapping as SchemaLoaderBase);
    }

    [Test]
    public void LoadSchemaSetWithMapping ()
    {
      XmlSchemaSet schemaSet = _schemaLoaderWithMapping.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.MappingNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void LoadSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = _schemaLoaderWithQueries.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.QueryConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void LoadSchemaSetWithStorageProviders ()
    {
      XmlSchemaSet schemaSet = _schemaLoaderWithStorageProviders.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.StorageProviderConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }
  }
}
