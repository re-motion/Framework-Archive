using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ClassDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _orderClass;
  private ClassDefinition _distributorClass;

  // construction and disposing

  public ClassDefinitionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
    _distributorClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];
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
      "Type 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassNotDerivedFromDomainObject'"
      + " of class 'Company' is not derived from"
      + " 'Rubicon.Data.DomainObjects.DomainObject'.")]
  public void ClassTypeWithInvalidDerivation ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "Company", "Company", typeof (ClassNotDerivedFromDomainObject), "TestDomain"); 
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Type 'Rubicon.Data.DomainObjects.DomainObject' of class 'Company' is not derived from"
      + " 'Rubicon.Data.DomainObjects.DomainObject'.")]
  public void ClassTypeDomainObject ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "Company", "Company", typeof (DomainObject), "TestDomain"); 
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Entity name ('Customer') of class 'Customer' and entity name ('Company') of its " + 
          "base class 'Company' must be equal.")]
  public void InvalidEntityNameOfBaseClass ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    ClassDefinition customerClass = new ClassDefinition (
        "Customer", "Customer", typeof (Customer), "TestDomain", companyClass);    
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
  public void BaseClassWithDifferentStorageProvider ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "Provider 1");

    ClassDefinition customerClass = new ClassDefinition (
        "Customer", "Company", typeof (Customer), "Provider 2", companyClass);    
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Class 'Company' already contains the property 'Name'.")]
  public void AddDuplicateProperty ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    companyClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    companyClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Class 'Customer' already contains the property 'Name'.")]
  public void AddDuplicatePropertyBaseClass ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");
    companyClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    ClassDefinition customerClass = new ClassDefinition (
        "Customer", "Company", typeof (Customer), "TestDomain", companyClass);    

    customerClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Class 'Supplier' already contains the property 'Name'.")]
  public void AddDuplicatePropertyBaseOfBaseClass ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");
    companyClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    ClassDefinition partnerClass = new ClassDefinition (
        "Partner", "Company", typeof (Partner), "TestDomain", companyClass);    

    ClassDefinition supplierClass = new ClassDefinition (
        "Supplier", "Company", typeof (Supplier), "TestDomain", partnerClass);    

    supplierClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
  }

  [Test]
  public void ConstructorWithoutBaseClass ()
  {
    ClassDefinition companyClass = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "No relation found for class 'Order' and property 'UndefinedProperty'.")]
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
  public void IsRelationEndPointWithNullRelationEndPointDefinition ()
  {
    ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"];

    NullRelationEndPointDefinition clientNullEndPointDefinition = (NullRelationEndPointDefinition)
        MappingConfiguration.Current.RelationDefinitions["ParentClientToChildClient"].GetEndPointDefinition ("Client", null);

    Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientNullEndPointDefinition));
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
  [ExpectedException (typeof (MappingException), "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
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
  [ExpectedException(typeof (MappingException), "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
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
  [ExpectedException (typeof (MappingException), "No relation found for class 'Order' and property 'InvalidProperty'.")]
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
  [ExpectedException (typeof (MappingException), "Class 'Order' does not contain the property 'InvalidProperty'.")]
  public void GetMandatoryPropertyDefinitionWithInvalidPropertName ()
  {
    _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
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
}
}
