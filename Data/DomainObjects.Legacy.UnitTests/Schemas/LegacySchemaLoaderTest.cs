using System;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Legacy.Schemas;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Schemas
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
    public void LoadSchemaSetWithMapping ()
    {
      XmlSchemaSet schemaSet = LegacySchemaLoader.Mapping.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (LegacyPrefixNamespace.MappingNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.re-motion.org/Data/DomainObjects/Types"));
    }
  }
}
