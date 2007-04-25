using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class XmlBasedClassDefinitionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private XmlBasedClassDefinition _orderClass;
    private XmlBasedClassDefinition _distributorClass;
    private ClassDefinitionChecker _checker;

    // construction and disposing

    public XmlBasedClassDefinitionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _orderClass = (XmlBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _distributorClass = (XmlBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];

      _checker = new ClassDefinitionChecker ();
    }

    [Test]
    public void InitializeWithTypeName ()
    {
      XmlBasedClassDefinition expected = new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order));

      XmlBasedClassDefinition actual = new XmlBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider",
          "Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain.Order, Rubicon.Data.DomainObjects.Legacy.UnitTests", true);

      _checker.Check (expected, actual);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InitializeWithTypeNotDerivedFromDomainObject ()
    {
      new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProvider", this.GetType ().AssemblyQualifiedName, true);
    }

    [Test]
    public void InitializeWithTypeNameAndBaseClass ()
    {
      XmlBasedClassDefinition expected = new XmlBasedClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor), CreatePartnerClass ());

      XmlBasedClassDefinition actual = new XmlBasedClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, 
          "Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain.Distributor, Rubicon.Data.DomainObjects.Legacy.UnitTests", true, CreatePartnerClass ());

      _checker.Check (expected, actual);
    }

    [Test]
    public void IntializeWithUnresolvedTypeName ()
    {
      XmlBasedClassDefinition actual = new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProvider", "UnexistingType", false);
      Assert.IsNull (actual.ClassType);
      Assert.AreEqual ("UnexistingType", actual.ClassTypeName);
      Assert.IsFalse (actual.IsClassTypeResolved);
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType ()
    {
      XmlBasedClassDefinition actual = new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order));

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType ()
    {
      XmlBasedClassDefinition actual = new XmlBasedClassDefinition ("AbstractClassNotInMapping", "Table", "StorageProvider", typeof (AbstractClassNotInMapping));

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot evaluate IsAbstract for ClassDefinition 'Order' since ResolveTypeNames is false.")]
    public void GetIsAbstract_ForUnresolvedTypeName ()
    {
      XmlBasedClassDefinition actual = new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProvider", "UnexistingType", false);
      Dev.Null = actual.IsAbstract;
    }

    [Test]
    public void GetRelationDefinition ()
    {
      RelationDefinition relation = _orderClass.GetRelationDefinition ("Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("CustomerToOrder", relation.ID);
    }

    [Test]
    public void GetUndefinedRelationDefinition ()
    {
      Assert.IsNull (_orderClass.GetRelationDefinition ("OrderNumber"));
    }

    [Test]
    public void GetAllRelationDefinitions ()
    {
      RelationDefinitionCollection relations = _distributorClass.GetRelationDefinitions ();

      Assert.IsNotNull (relations);
      Assert.AreEqual (5, relations.Count);
      Assert.IsNotNull (relations["CompanyToCeo"]);
      Assert.IsNotNull (relations["PartnerToPerson"]);
      Assert.IsNotNull (relations["DistributorToClassWithoutRelatedClassIDColumn"]);
      Assert.IsNotNull (relations["CompanyToClassWithoutRelatedClassIDColumnAndDerivation"]);
      Assert.IsNotNull (relations["IndustrialSectorToCompany"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyRelationDefinition ()
    {
      _orderClass.GetRelationDefinition (string.Empty);
    }

    [Test]
    public void GetRelationDefinitionWithInheritance ()
    {
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("ContactPerson"));
    }

    [Test]
    public void GetRelatedClassDefinition ()
    {
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("Ceo"));
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelatedClassDefinitionWithEmtpyPropertyName ()
    {
      _distributorClass.GetOppositeClassDefinition (string.Empty);
    }

    [Test]
    public void GetRelationEndPointDefinition ()
    {
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelationEndPointDefinitionFromEmptyPropertyName ()
    {
      _orderClass.GetRelationEndPointDefinition (string.Empty);
    }

    [Test]
    public void IsRelationEndPointTrue ()
    {
      RelationDefinition orderToOrderItem = TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"];
      IRelationEndPointDefinition endPointDefinition = orderToOrderItem.GetEndPointDefinition ("Order", "OrderItems");

      Assert.IsTrue (_orderClass.IsRelationEndPoint (endPointDefinition));
    }

    [Test]
    public void IsRelationEndPointFalse ()
    {
      RelationDefinition partnerToPerson = TestMappingConfiguration.Current.RelationDefinitions["PartnerToPerson"];
      IRelationEndPointDefinition partnerEndPoint = partnerToPerson.GetEndPointDefinition ("Partner", "ContactPerson");

      Assert.IsFalse (_orderClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsRelationEndPointWithNull ()
    {
      _orderClass.IsRelationEndPoint (null);
    }

    [Test]
    public void IsRelationEndPointWithInheritance ()
    {
      RelationDefinition partnerToPerson = TestMappingConfiguration.Current.RelationDefinitions["PartnerToPerson"];
      IRelationEndPointDefinition partnerEndPoint = partnerToPerson.GetEndPointDefinition ("Partner", "ContactPerson");

      Assert.IsTrue (_distributorClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      Assert.IsNotNull (_orderClass.GetPropertyDefinition ("OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyPropertyDefinition ()
    {
      _orderClass.GetPropertyDefinition (string.Empty);
    }

    [Test]
    public void GetInheritedPropertyDefinition ()
    {
      Assert.IsNotNull (_distributorClass.GetPropertyDefinition ("ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain.ClassNotDerivedFromDomainObject'"
        + " of class 'Company' is not derived from"
        + " 'Rubicon.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeWithInvalidDerivation ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (ClassNotDerivedFromDomainObject));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.DomainObject' of class 'Company' is not derived from"
        + " 'Rubicon.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeDomainObject ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (DomainObject));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void BaseClassWithDifferentStorageProvider ()
    {
      XmlBasedClassDefinition companyClass = new XmlBasedClassDefinition ("Company", "Company", "Provider 1", typeof (Company));

      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition (
          "Customer", "Company", "Provider 2", typeof (Customer), companyClass);
    }

    [Test]
    public void ClassTypeIsNotDerivedFromBaseClassType ()
    {
      XmlBasedClassDefinition orderClass = new XmlBasedClassDefinition ("Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));

      try
      {
        new XmlBasedClassDefinition ("Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor), orderClass);
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.", 
            typeof (Distributor).AssemblyQualifiedName, "Distributor", orderClass.ClassType.AssemblyQualifiedName, orderClass.ID);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ClassTypeIsNotDerivedFromBaseClassTypeWithUnresolvedTypes ()
    {
      XmlBasedClassDefinition orderClass = new XmlBasedClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order).AssemblyQualifiedName, false);

      XmlBasedClassDefinition distributorClass = new XmlBasedClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor).AssemblyQualifiedName, false, orderClass);

      // Expectation: no exception
    }

    [Test]
    public void ClassTypeIsNotDerivedFromBaseClassTypeWithUnresolvedTypeInBaseClassOnly ()
    {
      XmlBasedClassDefinition orderClass = new XmlBasedClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order).AssemblyQualifiedName, false);

      XmlBasedClassDefinition distributorClass = new XmlBasedClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor).AssemblyQualifiedName, true, orderClass);

      // Expectation: no exception
    }
    
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Proeprty 'Name' cannot be added to class 'Company', because it already defines a property with the same name.")]
    public void AddDuplicateProperty ()
    {
      XmlBasedClassDefinition companyClass = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      companyClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));
      companyClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Proeprty 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass ()
    {
      XmlBasedClassDefinition companyClass = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));
      companyClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));

      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition ("Customer", "Company", "TestDomain", typeof (Customer), companyClass);
      customerClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Proeprty 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass ()
    {
      XmlBasedClassDefinition companyClass = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));
      companyClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));

      XmlBasedClassDefinition partnerClass = new XmlBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), companyClass);

      XmlBasedClassDefinition supplierClass = new XmlBasedClassDefinition ("Supplier", "Company", "TestDomain", typeof (Supplier), partnerClass);
      supplierClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition ("Name", "Name", "string", 100));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The PropertyDefinition 'PropertyName' cannot be added to ClassDefinition 'ClassID', "
        + "because the PropertyDefinition's type is resolved and the ClassDefinition's type is not.")]
    public void AddPropertyDefinitionWithResolvedTypeToClassDefinitionWithUnresolvedType ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassID", "Entity", "StorageProvider", "UnresolvedType", false);
      PropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "string", true, false, 100);

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The PropertyDefinition 'PropertyName' cannot be added to ClassDefinition 'ClassID', "
        + "because the ClassDefinition's type is resolved and the PropertyDefinition's type is not.")]
    public void AddPropertyDefinitionWithUnresolvedTypeToClassDefinitionWithResolvedType ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassID", "Entity", "StorageProvider", typeof (Order));
      PropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition ("PropertyName", "ColumnName", "UnresolvedType", false, false, 100);

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    public void ConstructorWithoutBaseClass ()
    {
      XmlBasedClassDefinition companyClass = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "No relation found for class 'Order' and property 'UndefinedProperty'.")]
    public void GetMandatoryRelationEndPointDefinitionForUndefinedProperty ()
    {
      _orderClass.GetMandatoryRelationEndPointDefinition ("UndefinedProperty");
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition ()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition = _orderClass.GetMandatoryOppositeEndPointDefinition ("OrderTicket");
      Assert.IsNotNull (oppositeEndPointDefinition);
      Assert.AreEqual ("Order", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions ()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions ();

      IRelationEndPointDefinition customerEndPoint = _orderClass.GetRelationEndPointDefinition ("Customer");
      IRelationEndPointDefinition orderTicketEndPoint = _orderClass.GetRelationEndPointDefinition ("OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint = _orderClass.GetRelationEndPointDefinition ("OrderItems");
      IRelationEndPointDefinition officialEndPoint = _orderClass.GetRelationEndPointDefinition ("Official");

      Assert.AreEqual (4, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, customerEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderTicketEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderItemsEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, officialEndPoint) >= 0);
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetMyRelationEndPointDefinitions ();

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (1, relationEndPointDefinitions.Length);
      Assert.AreEqual ("ClassWithoutRelatedClassIDColumn", relationEndPointDefinitions[0].PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance ()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions ();

      IRelationEndPointDefinition classWithoutRelatedClassIDColumnEndPoint = _distributorClass.GetRelationEndPointDefinition ("ClassWithoutRelatedClassIDColumn");
      IRelationEndPointDefinition contactPersonEndPoint = _distributorClass.GetRelationEndPointDefinition ("ContactPerson");
      IRelationEndPointDefinition ceoEndPoint = _distributorClass.GetRelationEndPointDefinition ("Ceo");
      IRelationEndPointDefinition classWithoutRelatedClassIDColumnAndDerivationEndPoint = _distributorClass.GetRelationEndPointDefinition ("ClassWithoutRelatedClassIDColumnAndDerivation");
      IRelationEndPointDefinition industrialSectorEndPoint = _distributorClass.GetRelationEndPointDefinition ("IndustrialSector");

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (5, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, contactPersonEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, ceoEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnAndDerivationEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, industrialSectorEndPoint) >= 0);
    }

    [Test]
    public void GetDerivedClassesWithoutInheritance ()
    {
      Assert.IsNotNull (_orderClass.DerivedClasses);
      Assert.AreEqual (0, _orderClass.DerivedClasses.Count);
      Assert.IsTrue (_orderClass.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void GetDerivedClassesWithInheritance ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions["Company"];

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Partner"));
      Assert.IsTrue (companyDefinition.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions["Company"];

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"];

      RelationDefinitionCollection clientRelations = clientDefinition.GetRelationDefinitions ();

      Assert.AreEqual (1, clientRelations.Count);
      Assert.AreEqual ("ParentClientToChildClient", clientRelations[0].ID);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"];

      AnonymousRelationEndPointDefinition clientAnonymousEndPointDefinition = (AnonymousRelationEndPointDefinition)
          MappingConfiguration.Current.RelationDefinitions["ParentClientToChildClient"].GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"];

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.GetMyRelationEndPointDefinitions ();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "ParentClient"));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetMyRelationEndPointDefinitions ();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      IRelationEndPointDefinition folderEndPoint = folderDefinition.GetRelationEndPointDefinition ("FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint = folderDefinition.GetRelationEndPointDefinition ("ParentFolder");

      Assert.IsTrue (folderDefinition.IsMyRelationEndPoint (folderEndPoint));
      Assert.IsFalse (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetMyRelationEndPointDefinitions ();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions ();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions ();

      Assert.AreEqual (2, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "FileSystemItems"));
      Assert.IsTrue (Contains (endPointDefinitions, "ParentFolder"));
    }

    [Test]
    public void GetRelationDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      RelationDefinitionCollection relations = fileSystemItemDefinition.GetRelationDefinitions ();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["FolderToFileSystemItem"]);
    }

    [Test]
    public void GetRelationDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      RelationDefinitionCollection relations = folderDefinition.GetRelationDefinitions ();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["FolderToFileSystemItem"]);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.IsNotNull (fileSystemItemDefinition.GetRelationEndPointDefinition ("ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetRelationEndPointDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      Assert.IsNotNull (folderDefinition.GetRelationEndPointDefinition ("ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationEndPointDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.IsNotNull (fileSystemItemDefinition.GetRelationDefinition ("ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetRelationDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      Assert.AreSame (folderDefinition, fileSystemItemDefinition.GetOppositeClassDefinition ("ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetOppositeClassDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.AreSame (folderDefinition, folderDefinition.GetOppositeClassDefinition ("ParentFolder"));
      Assert.AreSame (fileSystemItemDefinition, folderDefinition.GetOppositeClassDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("ParentFolder"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.IsNotNull (fileSystemItemDefinition.GetOppositeEndPointDefinition ("ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetOppositeEndPointDefinition ("FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      Assert.IsNotNull (folderDefinition.GetMandatoryOppositeEndPointDefinition ("ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetMandatoryOppositeEndPointDefinition ("FileSystemItems"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeEndPointDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions["Folder"];

      Assert.AreSame (folderDefinition, fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions["FileSystemItem"];

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryRelationDefinition ()
    {
      RelationDefinition relation = _orderClass.GetMandatoryRelationDefinition ("Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("CustomerToOrder", relation.ID);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'Order' and property 'InvalidProperty'.")]
    public void GetMandatoryRelationDefinitionWithInvalidPropertyName ()
    {
      _orderClass.GetMandatoryRelationDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryPropertyDefinition ()
    {
      Assert.IsNotNull (_orderClass.GetMandatoryPropertyDefinition ("OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Order' does not contain the property 'InvalidProperty'.")]
    public void GetMandatoryPropertyDefinitionWithInvalidPropertName ()
    {
      _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
    }

    [Test]
    public void SetClassDefinitionOfPropertyDefinition ()
    {
      PropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition ("Test", "Test", "int32");
      Assert.IsNull (propertyDefinition.ClassDefinition);

      // Note: Never use a XmlBasedClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void CancelAddingOfPropertyDefinition ()
    {
      PropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition ("Test", "Test", "int32");
      Assert.IsNull (propertyDefinition.ClassDefinition);

      // Note: Never use a XmlBasedClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));

      PropertyDefinitionCollectionEventReceiver receiver = new PropertyDefinitionCollectionEventReceiver (classDefinition.MyPropertyDefinitions, true);
      try
      {
        classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
        Assert.Fail ("Expected an EventReceiverCancelException.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsNull (propertyDefinition.ClassDefinition);
      }
    }

    [Test]
    public void Contains ()
    {
      Assert.IsFalse (_orderClass.Contains (new XmlBasedPropertyDefinition ("PropertyName", "ColumnName", "int32")));
      Assert.IsTrue (_orderClass.Contains (_orderClass["OrderNumber"]));
    }

    [Test]
    public void PropertyDefinitionCollectionBackLink ()
    {
      Assert.AreSame (_orderClass, _orderClass.MyPropertyDefinitions.ClassDefinition);
    }

    [Test]
    public void GetInheritanceRootClass ()
    {
      ClassDefinition expected = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass ());
    }

    [Test]
    public void GetAllDerivedClasses ()
    {
      ClassDefinition companyClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      ClassDefinitionCollection allDerivedClasses = companyClass.GetAllDerivedClasses ();
      Assert.IsNotNull (allDerivedClasses);
      Assert.AreEqual (4, allDerivedClasses.Count);

      Assert.IsTrue (allDerivedClasses.Contains (typeof (Customer)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Partner)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Supplier)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Distributor)));
    }

    [Test]
    public void IsSameOrBaseClassOfFalse ()
    {
      Assert.IsFalse (_orderClass.IsSameOrBaseClassOf (_distributorClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithSameClass ()
    {
      Assert.IsTrue (_orderClass.IsSameOrBaseClassOf (_orderClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithBaseClass ()
    {
      ClassDefinition companyClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      Assert.IsTrue (companyClass.IsSameOrBaseClassOf (_distributorClass));
    }

    private bool Contains (IRelationEndPointDefinition[] endPointDefinitions, string propertyName)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in endPointDefinitions)
      {
        if (endPointDefinition.PropertyName == propertyName)
          return true;
      }

      return false;
    }

    private XmlBasedClassDefinition CreatePartnerClass ()
    {
      return new XmlBasedClassDefinition ("Partner", "Company", DatabaseTest.c_testDomainProviderID, typeof (Partner));
    }
  }
}
