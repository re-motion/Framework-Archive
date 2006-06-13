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
  public class MappingLoaderTest : StandardMappingTest
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

    public override void SetUp ()
    {
      base.SetUp ();

      _loader = new MappingLoader (@"mapping.xml", true);
    }

    [Test]
    public void InitializeWithResolveTypes ()
    {
      string configurationFile = Path.GetFullPath (@"mapping.xml");

      Assert.AreEqual (configurationFile, _loader.ConfigurationFile);
      Assert.IsTrue (_loader.ResolveTypes);
    }

    [Test]
    public void InitializeBaseLoaderWithResolveTypes ()
    {
      Assert.IsTrue (((BaseFileLoader) _loader).ResolveTypes);
    }

    [Test]
    public void LoadingOfClassDefinitions ()
    {
      ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions ();

      ClassDefinitionChecker checker = new ClassDefinitionChecker ();
      checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
    }

    [Test]
    public void LoadWithUnresolvedTypes ()
    {
      MappingLoader loader = new MappingLoader ("mappingWithUnresolvedTypes.xml", false);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
      Assert.IsFalse (classDefinitions.AreResolvedTypesRequired);
    }

    [Test]
    public void LoadingOfRelations ()
    {
      ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions ();
      RelationDefinitionCollection actualRelationDefinitions = _loader.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions);
    }

    [Test]
    public void ReadAndValidateMappingFile ()
    {
      MappingLoader loader = new MappingLoader (@"mapping.xml", true);

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Class 'Company' cannot refer to itself as base class.")]
    public void MappingWithInvalidBaseClass ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithInvalidDerivation.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void MappingWithInvalidDerivationAcrossStorageProviders ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithInvalidDerivationAcrossStorageProviders.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Class 'Customer' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseAndDerivedClass ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithPropertyDefinedInBaseAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Class 'Supplier' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    public void MappingWithMinimumData ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithMinimumData.xml", true);

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
      MappingLoader loader = new MappingLoader (@"mappingWithNonExistingBaseClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithSchemaException ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithSchemaException.xml", true);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Error while reading mapping:"
       + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 4.")]
    public void MappingWithXmlException ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithXmlException.xml", true);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "RelationProperty 'OrderTicket' of relation 'OrderToOrderTicket' must not contain element 'collectionType'."
        + " Element 'collectionType' is only valid for relation properties with cardinality equal to 'many'.")]
    public void MappingWithCollectionTypeAndOneToOneRelation ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithCollectionTypeAndOneToOneRelation.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnName ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithDuplicateColumnName.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnNameAndRelationProperty ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithDuplicateColumnNameAndRelationProperty.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. For relations with only one relation property the relation property must define the opposite class.")]
    public void MappingWithOnlyOneEndPoint ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithOnlyOneEndPoint.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Property 'Order' of relation 'OrderToOrderTicket' defines a column and a cardinality equal to 'many', which is not valid.")]
    public void MappingWithColumnAndCardinalityMany ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithColumnAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Both property names of relation 'OrderToOrderTicket' are 'OrderTicket', which is not valid.")]
    public void MappingWithRelationAndIdenticalPropertyNames ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithRelationAndIdenticalPropertyNames.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Property 'OtherName' of class 'Customer' must not define column name 'NameColumn',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnName ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithDerivationAndDuplicateColumnName.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Property 'OtherName' of class 'Customer' must not define column name 'NameColumn',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnNameWithoutResolvedTypes ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithDerivationAndDuplicateColumnName.xml", false);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Property 'SupplierName' of class 'Supplier' must not define column name 'Name',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    public void GetApplicationName ()
    {
      MappingLoader loader = new MappingLoader (@"mapping.xml", true);
      Assert.AreEqual ("UnitTests", loader.GetApplicationName ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToCustomer' is not correctly defined. A relation must either have exactly two relation properties or the relation property must"
        + " have an opposite class defined.")]
    public void MappingWithMoreThanTwoEndPoints ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithMoreThanTwoEndPoints.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. A relation property with a cardinality of 'many' cannot define an opposite class.")]
    public void MappingWithOppositeClassAndCardinalityMany ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithOppositeClassAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. Because the relation is bidirectional the relation property 'Customer' must not define its opposite class.")]
    public void MappingWithOppositeClassAndTwoRelationProperties ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithOppositeClassAndTwoRelationProperties.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithPropertyDefinedInTwoBaseClasses ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithPropertyDefinedInTwoBaseClasses.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Error while reading mapping: The root element has namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidMappingNamespace'"
        + " but was expected to have 'http://www.rubicon-it.com/Data/DomainObjects/Mapping/1.0'.")]
    public void MappingWithInvalidNamespace ()
    {
      MappingLoader loader = new MappingLoader (@"mappingWithInvalidNamespace.xml", true);
    }
  }
}