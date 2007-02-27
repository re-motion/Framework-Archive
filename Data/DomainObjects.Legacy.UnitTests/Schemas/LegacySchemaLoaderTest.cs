using System;
using System.Xml.Schema;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.Schemas;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Schemas
{
  [TestFixture]
  public class LegacySchemaLoaderTest
  {
    [Test]
    public void InitializeWithMapping ()
    {
      Assert.AreEqual (LegacyPrefixNamespace.MappingNamespace.Uri, LegacySchemaLoader.Mapping.SchemaUri);
    }

    [Test]
    public void InitializeWithStorageProviders ()
    {
      Assert.AreEqual (LegacyPrefixNamespace.StorageProviderConfigurationNamespace.Uri, LegacySchemaLoader.StorageProviders.SchemaUri);
    }

    [Test]
    public void LoadSchemaSetWithMapping ()
    {
      XmlSchemaSet schemaSet = LegacySchemaLoader.Mapping.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (LegacyPrefixNamespace.MappingNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }

    [Test]
    public void LoadSchemaSetWithStorageProviders ()
    {
      XmlSchemaSet schemaSet = LegacySchemaLoader.StorageProviders.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (LegacyPrefixNamespace.StorageProviderConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.rubicon-it.com/Data/DomainObjects/Types"));
    }
  }
}
