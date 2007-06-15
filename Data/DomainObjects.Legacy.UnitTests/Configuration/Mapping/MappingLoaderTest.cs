using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingLoaderTest: StandardMappingTest
  {
    private MappingLoader _loader;
    private FakeConfigurationWrapper _configurationWrapper;

    public override void SetUp()
    {
      base.SetUp();

      _loader = new MappingLoader (@"Mapping.xml", true);

      _configurationWrapper = new FakeConfigurationWrapper ();
      _configurationWrapper.SetUpConnectionString ("Rdbms", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent (_configurationWrapper);
    }

    [Test]
    public void InitializeWithConfigurationFileFromAppSettings()
    {
      _configurationWrapper.SetUpAppSetting ("Rubicon.Data.DomainObjects.Mapping.ConfigurationFile", "DataDomainObjectsLegacy_MappingWithMinimumData.xml");
      MappingLoader loader = new MappingLoader();
      string configurationFile = Path.GetFullPath (@"DataDomainObjectsLegacy_MappingWithMinimumData.xml");

      Assert.AreEqual (configurationFile, loader.ConfigurationFile);
      Assert.IsTrue (loader.ResolveTypes);
    }

    [Test]
    public void InitializeWithDefaultFile()
    {
      MappingLoader loader = new MappingLoader();
      string configurationFile = Path.GetFullPath (@"Mapping.xml");

      Assert.AreEqual (configurationFile, loader.ConfigurationFile);
      Assert.IsTrue (loader.ResolveTypes);
    }

    [Test]
    public void InitializeWithResolveTypes()
    {
      string configurationFile = Path.GetFullPath (@"Mapping.xml");

      Assert.AreEqual (configurationFile, _loader.ConfigurationFile);
      Assert.IsTrue (_loader.ResolveTypes);
    }

    [Test]
    public void InitializeBaseLoaderWithResolveTypes()
    {
      Assert.IsTrue (((BaseFileLoader) _loader).ResolveTypes);
    }

    [Test]
    public void IMappingLoader_InitializeWithResolveTypes()
    {
      Assert.IsTrue (((IMappingLoader) _loader).ResolveTypes);
    }

    [Test]
    public void LoadingOfClassDefinitions()
    {
      ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions();

      ClassDefinitionChecker checker = new ClassDefinitionChecker();
      checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
    }

    [Test]
    public void IMappingLoader_LoadingOfClassDefinitions()
    {
      IMappingLoader iMappingLoader = _loader;
      ClassDefinitionCollection actualClassDefinitions = iMappingLoader.GetClassDefinitions();

      ClassDefinitionChecker checker = new ClassDefinitionChecker();
      checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
    }

    [Test]
    public void LoadWithUnresolvedTypes()
    {
      MappingLoader loader = new MappingLoader ("DataDomainObjectsLegacy_MappingWithUnresolvedTypes.xml", false);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
      Assert.IsFalse (classDefinitions.AreResolvedTypesRequired);
    }

    [Test]
    public void LoadingOfRelations()
    {
      ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions();
      RelationDefinitionCollection actualRelationDefinitions = _loader.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions);
    }

    [Test]
    public void IMappingLoader_LoadingOfRelations()
    {
      IMappingLoader iMappingLoader = _loader;
      ClassDefinitionCollection actualClassDefinitions = iMappingLoader.GetClassDefinitions();
      RelationDefinitionCollection actualRelationDefinitions = iMappingLoader.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions);
    }

    [Test]
    public void ReadAndValidateMappingFile()
    {
      MappingLoader loader = new MappingLoader (@"Mapping.xml", true);

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithInvalidBaseClass()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithInvalidDerivation.xml", true);

      try
      {
        ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.", 
            typeof (Company).AssemblyQualifiedName, "Company", typeof (Company).AssemblyQualifiedName, "Company");

        Assert.AreEqual (expectedMessage, ex.Message);
        throw;
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void MappingWithInvalidDerivationAcrossStorageProviders()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithInvalidDerivationAcrossStorageProviders.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Class 'Customer' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseAndDerivedClass()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithPropertyDefinedInBaseAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Class 'Supplier' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
    }

    [Test]
    public void MappingWithMinimumData()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithMinimumData.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
      RelationDefinitionCollection relationDefinitions = loader.GetRelationDefinitions (classDefinitions);

      Assert.IsNotNull (classDefinitions);
      Assert.AreEqual (0, classDefinitions.Count);
      Assert.IsNotNull (relationDefinitions);
      Assert.AreEqual (0, relationDefinitions.Count);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Class 'Customer' refers to non-existing base class 'NonExistingClass'.")]
    public void MappingWithNonExistingBaseClass()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithNonExistingBaseClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithSchemaException()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithSchemaException.xml", true);
    }

    [Test]
    public void MappingWithXmlException()
    {
      string configurationFile = "DataDomainObjectsLegacy_MappingWithXmlException.xml";
      try
      {
        MappingLoader loader = new MappingLoader (configurationFile, true);

        Assert.Fail ("MappingException was expected");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading mapping: '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 4. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "RelationProperty 'OrderTicket' of relation 'OrderToOrderTicket' must not contain element 'collectionType'."
            + " Element 'collectionType' is only valid for relation properties with cardinality equal to 'many'.")]
    public void MappingWithCollectionTypeAndOneToOneRelation()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithCollectionTypeAndOneToOneRelation.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnName()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithDuplicateColumnName.xml", true);

      loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnNameAndRelationProperty()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithDuplicateColumnNameAndRelationProperty.xml", true);

      loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The relation 'CustomerToOrder' is not correctly defined. "
        + "For relations with only one relation property the relation property must define the opposite class.")]
    public void MappingWithOnlyOneEndPoint()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithOnlyOneEndPoint.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Property 'Order' of relation 'OrderToOrderTicket' defines a column and a cardinality equal to 'many', which is not valid.")]
    public void MappingWithColumnAndCardinalityMany()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithColumnAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Both property names of relation 'OrderToOrderTicket' are 'OrderTicket', which is not valid.")]
    public void MappingWithRelationAndIdenticalPropertyNames()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithRelationAndIdenticalPropertyNames.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
       ExpectedMessage = "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn',"
           + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void MappingWithDerivationAndDuplicateColumnName()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithDerivationAndDuplicateColumnName.xml", true);

      loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
       ExpectedMessage = "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn',"
           + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void MappingWithDerivationAndDuplicateColumnNameWithoutResolvedTypes()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithDerivationAndDuplicateColumnName.xml", false);

      loader.GetClassDefinitions();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
       ExpectedMessage = "Property 'SupplierName' of class 'Supplier' must not define storage specific name 'Name',"
           + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void MappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass.xml", true);

      loader.GetClassDefinitions();
    }

    [Test]
    public void GetApplicationName()
    {
      MappingLoader loader = new MappingLoader (@"Mapping.xml", true);
      Assert.AreEqual ("UnitTests", loader.GetApplicationName());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The relation 'CustomerToCustomer' is not correctly defined. "
        + "A relation must either have exactly two relation properties or the relation property must have an opposite class defined.")]
    public void MappingWithMoreThanTwoEndPoints()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithMoreThanTwoEndPoints.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The relation 'CustomerToOrder' is not correctly defined. A relation property with a cardinality of 'many' cannot define an opposite class.")]
    public void MappingWithOppositeClassAndCardinalityMany()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithOppositeClassAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The relation 'CustomerToOrder' is not correctly defined. "
        + "Because the relation is bidirectional the relation property 'Customer' must not define its opposite class.")]
    public void MappingWithOppositeClassAndTwoRelationProperties()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithOppositeClassAndTwoRelationProperties.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithPropertyDefinedInTwoBaseClasses()
    {
      MappingLoader loader = new MappingLoader (@"DataDomainObjectsLegacy_MappingWithPropertyDefinedInTwoBaseClasses.xml", true);

      loader.GetClassDefinitions();
    }

    [Test]
    public void MappingWithInvalidNamespace()
    {
      string configurationFile = "DataDomainObjectsLegacy_MappingWithInvalidNamespace.xml";
      try
      {
        MappingLoader loader = new MappingLoader (configurationFile, true);

        Assert.Fail ("MappingException was expected");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading mapping: The namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidNamespace' of the root element is invalid."
                + " Expected namespace: 'http://www.rubicon-it.com/Data/DomainObjects/Mapping/1.0'. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void MappingWithUnorderedElements()
    {
      MappingLoader loader = new MappingLoader ("DataDomainObjectsLegacy_MappingWithUnorderedElements.xml", true);

      loader.GetClassDefinitions();

      // expectation: no exception
    }
  }
}