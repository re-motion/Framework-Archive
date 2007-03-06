using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
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

      _loader = new MappingLoader (@"Mapping.xml", true);
    }

    [Test]
    public void InitializeWithResolveTypes ()
    {
      string configurationFile = Path.GetFullPath (@"Mapping.xml");

      Assert.AreEqual (configurationFile, _loader.ConfigurationFile);
      Assert.IsTrue (_loader.ResolveTypes);
    }

    [Test]
    public void InitializeBaseLoaderWithResolveTypes ()
    {
      Assert.IsTrue (((BaseFileLoader) _loader).ResolveTypes);
    }

    [Test]
    public void IMappingLoader_InitializeWithResolveTypes ()
    {
      Assert.IsTrue (((IMappingLoader) _loader).ResolveTypes);
    }

    [Test]
    public void LoadingOfClassDefinitions ()
    {
      ClassDefinitionCollection actualClassDefinitions = _loader.GetClassDefinitions ();

      ClassDefinitionChecker checker = new ClassDefinitionChecker ();
      checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
    }

    [Test]
    public void IMappingLoader_LoadingOfClassDefinitions ()
    {
      IMappingLoader iMappingLoader = _loader;
      ClassDefinitionCollection actualClassDefinitions = iMappingLoader.GetClassDefinitions ();

      ClassDefinitionChecker checker = new ClassDefinitionChecker ();
      checker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false);
    }

    [Test]
    public void LoadWithUnresolvedTypes ()
    {
      MappingLoader loader = new MappingLoader ("MappingWithUnresolvedTypes.xml", false);

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
    public void IMappingLoader_LoadingOfRelations ()
    {
      IMappingLoader iMappingLoader = _loader;
      ClassDefinitionCollection actualClassDefinitions = iMappingLoader.GetClassDefinitions ();
      RelationDefinitionCollection actualRelationDefinitions = iMappingLoader.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions);
    }

    [Test]
    public void ReadAndValidateMappingFile ()
    {
      MappingLoader loader = new MappingLoader (@"Mapping.xml", true);

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Class 'Company' cannot refer to itself as base class.")]
    public void MappingWithInvalidBaseClass ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithInvalidDerivation.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void MappingWithInvalidDerivationAcrossStorageProviders ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithInvalidDerivationAcrossStorageProviders.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Class 'Customer' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseAndDerivedClass ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithPropertyDefinedInBaseAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Class 'Supplier' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithPropertyDefinedInBaseOfBaseClassAndDerivedClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    public void MappingWithMinimumData ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithMinimumData.xml", true);

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
      MappingLoader loader = new MappingLoader (@"MappingWithNonExistingBaseClass.xml", true);

      ClassDefinitionCollection classDefinitions = loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithSchemaException ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithSchemaException.xml", true);
    }

    [Test]
    public void MappingWithXmlException ()
    {
      string configurationFile = "MappingWithXmlException.xml";
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
        "RelationProperty 'OrderTicket' of relation 'OrderToOrderTicket' must not contain element 'collectionType'."
        + " Element 'collectionType' is only valid for relation properties with cardinality equal to 'many'.")]
    public void MappingWithCollectionTypeAndOneToOneRelation ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithCollectionTypeAndOneToOneRelation.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnName ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithDuplicateColumnName.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithDuplicateColumnNameAndRelationProperty ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithDuplicateColumnNameAndRelationProperty.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. For relations with only one relation property the relation property must define the opposite class.")]
    public void MappingWithOnlyOneEndPoint ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithOnlyOneEndPoint.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Property 'Order' of relation 'OrderToOrderTicket' defines a column and a cardinality equal to 'many', which is not valid.")]
    public void MappingWithColumnAndCardinalityMany ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithColumnAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Both property names of relation 'OrderToOrderTicket' are 'OrderTicket', which is not valid.")]
    public void MappingWithRelationAndIdenticalPropertyNames ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithRelationAndIdenticalPropertyNames.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Property 'OtherName' of class 'Customer' must not define column name 'NameColumn',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnName ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithDerivationAndDuplicateColumnName.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Property 'OtherName' of class 'Customer' must not define column name 'NameColumn',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnNameWithoutResolvedTypes ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithDerivationAndDuplicateColumnName.xml", false);

      loader.GetClassDefinitions ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Property 'SupplierName' of class 'Supplier' must not define column name 'Name',"
        + " because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same column name.")]
    public void MappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithDerivationAndDuplicateColumnNameInBaseOfBaseClass.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    public void GetApplicationName ()
    {
      MappingLoader loader = new MappingLoader (@"Mapping.xml", true);
      Assert.AreEqual ("UnitTests", loader.GetApplicationName ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToCustomer' is not correctly defined. A relation must either have exactly two relation properties or the relation property must"
        + " have an opposite class defined.")]
    public void MappingWithMoreThanTwoEndPoints ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithMoreThanTwoEndPoints.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. A relation property with a cardinality of 'many' cannot define an opposite class.")]
    public void MappingWithOppositeClassAndCardinalityMany ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithOppositeClassAndCardinalityMany.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The relation 'CustomerToOrder' is not correctly defined. Because the relation is bidirectional the relation property 'Customer' must not define its opposite class.")]
    public void MappingWithOppositeClassAndTwoRelationProperties ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithOppositeClassAndTwoRelationProperties.xml", true);

      loader.GetRelationDefinitions (loader.GetClassDefinitions ());
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void MappingWithPropertyDefinedInTwoBaseClasses ()
    {
      MappingLoader loader = new MappingLoader (@"MappingWithPropertyDefinedInTwoBaseClasses.xml", true);

      loader.GetClassDefinitions ();
    }

    [Test]
    public void MappingWithInvalidNamespace ()
    {
      string configurationFile = "MappingWithInvalidNamespace.xml";
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
    public void MappingWithUnorderedElements ()
    {
      MappingLoader loader = new MappingLoader ("MappingWithUnorderedElements.xml", true);

      loader.GetClassDefinitions ();

      // expectation: no exception
    }
  }
}