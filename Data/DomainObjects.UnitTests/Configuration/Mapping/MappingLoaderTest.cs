using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class MappingLoaderTest
{
  // types

  // static members and constants

  // member fields
  private MappingLoader _loader;

  // construction and disposing

  public MappingLoaderTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void Setup ()
  {
    _loader = new MappingLoader (@"mapping.xml", @"mapping.xsd", true);
  }

  [Test]
  public void InitializeWithResolveTypeNames ()
  {
    MappingLoader loader = new MappingLoader (@"mapping.xml", @"mapping.xsd", true);

    string configurationFile = Path.GetFullPath (@"mapping.xml");
    string schemaFile = Path.GetFullPath (@"mapping.xsd");

    Assert.AreEqual (configurationFile, loader.ConfigurationFile);
    Assert.AreEqual (schemaFile, loader.SchemaFile);
    Assert.IsTrue (loader.ResolveTypeNames);
  }

  [Test]
  public void InitializeBaseLoaderWithResolveTypeNames ()
  {
    MappingLoader loader = new MappingLoader (@"mapping.xml", @"mapping.xsd", true);

    Assert.IsTrue (((BaseLoader) loader).ResolveTypeNames);
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
    MappingLoader loader = new MappingLoader (@"mapping.xml", @"mapping.xsd", true);

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Class 'Company' cannot refer to itself as base class.")]
  public void MappingWithInvalidBaseClass ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithInvalidDerivation.xml", @"mapping.xsd", true);
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Entity name ('Customer') of class 'Customer' and entity name ('Company') of its " + 
          "base class 'Company' must be equal.")]
  public void MappingWithDerivationAndInvalidEntityName ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithDerivationAndInvalidEntityName.xml", @"mapping.xsd", true);
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
  public void MappingWithInvalidDerivationAcrossStorageProviders ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithInvalidDerivationAcrossStorageProviders.xml", @"mapping.xsd", true);
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Class 'Company' cannot be set as base class for class 'Customer',"
        + " because the property 'Name' is defined in both classes.")]
  public void MappingWithPropertyDefinedInBaseAndDerivedClass ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithPropertyDefinedInBaseAndDerivedClass.xml", @"mapping.xsd", true);
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Class 'Partner' cannot be set as base class for class 'Supplier',"
        + " because the property 'Name' is defined in both classes.")]
  public void MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass.xml", @"mapping.xsd", true);
    
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  public void MappingWithMinimumData ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithMinimumData.xml", @"mapping.xsd", true);
    
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
    MappingLoader loader = new MappingLoader (@"mappingWithNonExistingBaseClass.xml", @"mapping.xsd", true);
  
    ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void MappingWithSchemaException ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithSchemaException.xml", @"mapping.xsd", true);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Error while reading mapping:"
     + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 4.")]
  public void MappingWithXmlException ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithXmlException.xml", @"mapping.xsd", true);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "RelationProperty 'OrderTicket' of relation 'OrderToOrderTicket' must not contain element 'collectionType'."
      + " Element 'collectionType' is only valid for relation properties with cardinality equal to 'many'.")]
  public void MappingWithCollectionTypeAndOneToOneRelation ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithCollectionTypeAndOneToOneRelation.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void MappingWithDuplicateColumnName ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithDuplicateColumnName.xml", @"mapping.xsd", true);
  
    loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void MappingWithDuplicateColumnNameAndRelationProperty ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithDuplicateColumnNameAndRelationProperty.xml", @"mapping.xsd", true);
  
    loader.GetClassDefinitions ();
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The relation 'CustomerToOrder' is not correctly defined. For relations with only one relation property the relation property must define the opposite class.")]
  public void MappingWithOnlyOneEndPoint ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithOnlyOneEndPoint.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  [ExpectedException (typeof (MappingException), "Property 'Order' of relation 'OrderToOrderTicket' defines a column and a cardinality equal to 'many', which is not valid.")]
  public void MappingWithColumnAndCardinalityMany ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithColumnAndCardinalityMany.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  [ExpectedException (typeof (MappingException), "Both property names of relation 'OrderToOrderTicket' are 'OrderTicket', which is not valid.")]
  public void MappingWithRelationAndIdenticalPropertyNames ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithRelationAndIdenticalPropertyNames.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  [ExpectedException (typeof (MappingException), "Property 'SpecialCustomer' of class 'SpecialOrder' inherits a property which already defines the column 'CustomerID'.")]
  public void MappingWithDerivationAndDuplicateColumnName ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithDerivationAndDuplicateColumnName.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  public void GetApplicationName ()
  {
    MappingLoader loader = new MappingLoader (@"mapping.xml", @"mapping.xsd", true);
    Assert.AreEqual ("UnitTests", loader.GetApplicationName ());
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The relation 'CustomerToCustomer' is not correctly defined. A relation must either have exactly two relation properties or the relation property must"
      + " have an opposite class defined.")]
  public void MappingWithMoreThanTwoEndPoints ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithMoreThanTwoEndPoints.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The relation 'CustomerToOrder' is not correctly defined. A relation property with a cardinality of 'many' cannot define an opposite class.")]
  public void MappingWithOppositeClassAndCardinalityMany ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithOppositeClassAndCardinalityMany.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The relation 'CustomerToOrder' is not correctly defined. Because the relation is bidirectional the relation property 'Customer' must not define its opposite class.")]
  public void MappingWithOppositeClassAndTwoRelationProperties ()
  {
    MappingLoader loader = new MappingLoader (@"mappingWithOppositeClassAndTwoRelationProperties.xml", @"mapping.xsd", true);
  
    loader.GetRelationDefinitions (loader.GetClassDefinitions ());
  } 
}
}