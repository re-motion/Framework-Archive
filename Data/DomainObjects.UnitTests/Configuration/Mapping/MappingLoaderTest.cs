using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class LoaderTest
{
  // types

  // static members and constants

  // member fields
  private MappingLoader _loader;

  // construction and disposing

  public LoaderTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void Setup ()
  {
    _loader = new MappingLoader (@"mapping.xml", @"mapping.xsd");
  }

  [Test]
  public void TypeMapping ()
  {
    string[] types = new string[]
        {
          "byte",
          "boolean", 
          "dateTime", 
          "decimal", 
          "double", 
          "guid", 
          "int16", 
          "int32", 
          "int64", 
          "string", 
          "char", 
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.UnitTests",
          "objectID"
        };

    foreach (string type in types)
    {
      LoaderUtility.MapType (type);
    }
  }

  [Test]
  public void LoadingOfClassDefinitions ()
  {
    ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions ();
    
    ClassDefinitionChecker checker = new ClassDefinitionChecker ();
    checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
  }

  [Test]
  public void LoadingOfRelations ()
  {
    ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions ();
    RelationDefinitionCollection actualRelationDefinitions = _loader.GetRelationDefinitions  (actualClassDefinitions);

    ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
    classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, true);

    RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
    relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions);
  }

  [Test]
  public void ReadAndValidateMappingFile ()
  {
    MappingLoader loader = new MappingLoader (
        @"mapping.xml", 
        @"mapping.xsd");

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Class 'Company' cannot refer to itself as base class.")]
  public void MappingWithInvalidBaseClass ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithInvalidDerivation.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Entity name ('Customer') of class 'Customer' and entity name ('Company') of its " + 
          "base class 'Company' must be equal.")]
  public void MappingWithDerivationAndInvalidEntityName ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithDerivationAndInvalidEntityName.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
  public void MappingWithInvalidDerivationAcrossStorageProviders ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithInvalidDerivationAcrossStorageProviders.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Class 'Company' cannot be set as base class for class 'Customer',"
        + " because the property 'Name' is defined in both classes.")]
  public void MappingWithPropertyDefinedInBaseAndDerivedClass ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithPropertyDefinedInBaseAndDerivedClass.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Class 'Partner' cannot be set as base class for class 'Supplier',"
        + " because the property 'Name' is defined in both classes.")]
  public void MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  public void MappingWithMinimumData ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithMinimumData.xml", 
        @"mapping.xsd");
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    RelationDefinitionCollection relationDefinitions = loader.GetRelationDefinitions (classDefinitions);
 
    Assert.IsNotNull (classDefinitions);
    Assert.AreEqual (0, classDefinitions.Count);
    Assert.IsNotNull (relationDefinitions);
    Assert.AreEqual (0, relationDefinitions.Count);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Class 'Customer' refers to non-existing base class 'NonExistingClass'.")]
  public void MappingWithNonExistingBaseClass ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithNonExistingBaseClass.xml", 
        @"mapping.xsd");
  
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void MappingWithSchemaException ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithSchemaException.xml", 
        @"mapping.xsd");
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Error while reading mapping:"
     + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 4.")]
  public void MappingWithXmlException ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithXmlException.xml", 
        @"mapping.xsd");
  }


  [Test]
  public void ReadAndValidateStorageProviderFile ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProviders.xml", 
        @"storageProviders.xsd");

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (StorageProviderConfigurationException))]
  public void StorageProvidersWithSchemaException ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProvidersWithSchemaException.xml", 
        @"storageProviders.xsd");
  }

  [Test]
  [ExpectedException (typeof (StorageProviderConfigurationException), 
      "Error while reading storage provider configuration:"
      + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 3.")]
  public void StorageProvidersWithXmlException ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProvidersWithXmlException.xml", 
        @"storageProviders.xsd");
  }


  [Test]
  [ExpectedException (typeof (MappingException), 
      "Virtual end point of relation 'OrderToOrderTicket' must not contain element 'collectionType'."
      + " Element 'collectionType' is only valid for virtual end points with cardinality equal 'many'.")]
  public void MappingWithCollectionTypeAndOneToOneRelation ()
  {
    MappingLoader loader = new MappingLoader (
        @"mappingWithCollectionTypeAndOneToOneRelation.xml", 
        @"mapping.xsd");
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  }
}
}