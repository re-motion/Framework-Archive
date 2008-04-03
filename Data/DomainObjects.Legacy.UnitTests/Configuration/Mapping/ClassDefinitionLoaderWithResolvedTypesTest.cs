using System;
using System.Xml;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{

  [TestFixture]
  public class ClassDefinitionLoaderWithResolvedTypesTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionLoader _loader;

    // construction and disposing

    public ClassDefinitionLoaderWithResolvedTypesTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      XmlDocument mappingDocument = new XmlDocument ();
      mappingDocument.Load (@"DataDomainObjectsLegacy_MappingWithMinimumData.xml");

      PrefixNamespace[] namespaces = new PrefixNamespace[] { LegacyPrefixNamespace.MappingNamespace };
      ConfigurationNamespaceManager namespaceManager = new ConfigurationNamespaceManager (mappingDocument, namespaces);

      _loader = new ClassDefinitionLoader (mappingDocument, namespaceManager, true);
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsTrue (_loader.ResolveTypes);
    }
  }
}
