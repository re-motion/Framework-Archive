using System;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{

[TestFixture]
public class ClassDefinitionLoaderWithResolvedTypeNamesTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinitionLoader _loader;

  // construction and disposing

  public ClassDefinitionLoaderWithResolvedTypeNamesTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    XmlDocument mappingDocument = new XmlDocument ();
    mappingDocument.Load (@"mappingWithMinimumData.xml");

    PrefixNamespace[] namespaces = new PrefixNamespace[] {PrefixNamespace.MappingNamespace};
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
