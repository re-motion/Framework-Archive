using System;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

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
    Assert.AreEqual (4, classDefinitions.Count);
    
    ClassDefinition classDefinition = classDefinitions.GetMandatory ("Customer");
    Assert.IsFalse (classDefinition.IsClassTypeResolved);
    Assert.AreEqual ("UnresolvedCustomerType, UnknownAssembly", classDefinition.ClassTypeName);

    PropertyDefinitionCollection propertyDefinitions = classDefinition.GetPropertyDefinitions();
    Assert.AreEqual (2, propertyDefinitions.Count);

    PropertyDefinition customerSinceProperty = propertyDefinitions["CustomerSince"];
    Assert.IsFalse (customerSinceProperty.IsPropertyTypeResolved);
    Assert.AreEqual ("dateTime", customerSinceProperty.MappingTypeName);

    PropertyDefinition customerTypeProperty = propertyDefinitions["Type"];
    Assert.IsFalse (customerTypeProperty.IsPropertyTypeResolved);
    Assert.AreEqual ("UnresolvedCustomerType+UnresolvedCustomerEnum, UnknownAssembly", customerTypeProperty.MappingTypeName);

    ClassDefinition orderDefinition = classDefinitions.GetMandatory ("Order");
    PropertyDefinition customerProperty = orderDefinition.GetMandatoryPropertyDefinition ("Customer");
    Assert.IsFalse (customerProperty.IsPropertyTypeResolved);
    Assert.AreEqual (TypeInfo.ObjectIDMappingTypeName, customerProperty.MappingTypeName);
  }
}
}
