using System;
using System.Xml;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionLoaderWithUnresolvedTypesTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionLoader _loader;

    // construction and disposing

    public ClassDefinitionLoaderWithUnresolvedTypesTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      XmlDocument mappingDocument = new XmlDocument ();
      mappingDocument.Load (@"DataDomainObjectsLegacy_MappingWithUnresolvedTypes.xml");

      PrefixNamespace[] namespaces = new PrefixNamespace[] { LegacyPrefixNamespace.MappingNamespace };
      ConfigurationNamespaceManager namespaceManager = new ConfigurationNamespaceManager (mappingDocument, namespaces);

      _loader = new ClassDefinitionLoader (mappingDocument, namespaceManager, false);
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsFalse (_loader.ResolveTypes);
    }

    [Test]
    public void MappingWithUnresolvedTypes ()
    {
      ClassDefinitionCollection classDefinitions = _loader.GetClassDefinitions ();

      Assert.IsFalse (classDefinitions.AreResolvedTypesRequired);
      Assert.AreEqual (4, classDefinitions.Count);

      ClassDefinition classDefinition = classDefinitions.GetMandatory ("Customer");
      Assert.IsFalse (classDefinition.IsClassTypeResolved);
      Assert.AreEqual ("UnresolvedCustomerType, UnknownAssembly", classDefinition.ClassTypeName);

      PropertyDefinitionCollection propertyDefinitions = classDefinition.GetPropertyDefinitions ();
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
