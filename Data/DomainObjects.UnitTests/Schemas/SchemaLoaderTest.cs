using System;
using System.Xml.Schema;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.Schemas;
using Rubicon.Data.DomainObjects.Schemas;
using Rubicon.Xml;

namespace Rubicon.Data.DomainObjects.UnitTests.Schemas
{
  [TestFixture]
  public class SchemaLoaderTest
  {
    [Test]
    public void InitializeWithMapping ()
    {
      Assert.AreEqual (PrefixNamespace.MappingNamespace.Uri, LegacySchemaLoader.Mapping.SchemaUri);
    }

    [Test]
    public void InitializeWithQueries ()
    {
      Assert.AreEqual (PrefixNamespace.QueryConfigurationNamespace.Uri, SchemaLoader.Queries.SchemaUri);
    }

    [Test]
    public void InitializeWithStorageProviders ()
    {
      Assert.AreEqual (PrefixNamespace.StorageProviderConfigurationNamespace.Uri, LegacySchemaLoader.StorageProviders.SchemaUri);
    }

    [Test]
    public void IsDerivedFromSchemaBase ()
    {
      Assert.IsNotNull (SchemaLoader.Queries as SchemaLoaderBase);
    }

    [Test]
    public void LoadSchemaSetWithMapping ()
    {
      XmlSchemaSet schemaSet = LegacySchemaLoader.Mapping.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.MappingNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void LoadSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = SchemaLoader.Queries.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.QueryConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void LoadSchemaSetWithStorageProviders ()
    {
      XmlSchemaSet schemaSet = LegacySchemaLoader.StorageProviders.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.StorageProviderConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }
  }
}
