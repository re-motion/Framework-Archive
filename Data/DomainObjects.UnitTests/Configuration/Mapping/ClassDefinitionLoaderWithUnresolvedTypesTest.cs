using System;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{

[TestFixture]
public class ClassDefinitionLoaderWithUnresolvedTypeNamesTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinitionLoader _loader;

  // construction and disposing

  public ClassDefinitionLoaderWithUnresolvedTypeNamesTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    XmlDocument mappingDocument = new XmlDocument ();
    mappingDocument.Load (@"mappingWithUnresolvedTypes.xml");

    PrefixNamespace[] namespaces = new PrefixNamespace[] {PrefixNamespace.MappingNamespace};
    ConfigurationNamespaceManager namespaceManager = new ConfigurationNamespaceManager (mappingDocument, namespaces);
    
    _loader = new ClassDefinitionLoader (mappingDocument, namespaceManager, false);
  }

  [Test]
  public void Initialize ()
  {
    Assert.IsFalse (_loader.ResolveTypeNames);
  }
  
  [Test]
  public void MappingWithUnresolvedTypeNames ()
  {
    ClassDefinitionCollection classDefinitions = _loader.GetClassDefinitions ();

    Assert.IsFalse (classDefinitions.AreResolvedTypeNamesRequired);
    Assert.AreEqual (1, classDefinitions.Count);
    
    ClassDefinition classDefinition = classDefinitions.GetMandatory ("ClassWithUnresolvedTypes");
    Assert.IsFalse (classDefinition.IsClassTypeResolved);
    Assert.AreEqual ("UnknownClassType, Rubicon.Data.DomainObjects.UnitTests", classDefinition.ClassTypeName);

    PropertyDefinitionCollection propertyDefinitions = classDefinition.GetPropertyDefinitions();
    Assert.AreEqual (2, propertyDefinitions.Count);

    PropertyDefinition int32Property = propertyDefinitions["Int32Property"];
    Assert.IsFalse (int32Property.IsPropertyTypeResolved);
    Assert.AreEqual ("int32", int32Property.MappingTypeName);

    PropertyDefinition enumProperty = propertyDefinitions["EnumProperty"];
    Assert.IsFalse (enumProperty.IsPropertyTypeResolved);
    Assert.AreEqual ("UnknownClassType+EnumType, Rubicon.Data.DomainObjects.UnitTests", enumProperty.MappingTypeName);
  }
}
}
