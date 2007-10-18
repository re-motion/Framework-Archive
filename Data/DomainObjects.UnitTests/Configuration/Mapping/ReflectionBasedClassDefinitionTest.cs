using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _orderClass;
    private ReflectionBasedClassDefinition _distributorClass;

    public override void SetUp()
    {
      base.SetUp();

      _orderClass = (ReflectionBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _distributorClass = (ReflectionBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];
    }

    [Test]
    public void Initialize()
    {
      ClassDefinition actual = new ReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order), false, new List<Type>());

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.MyEntityName, Is.EqualTo ("OrderTable"));
      Assert.That (actual.StorageProviderID, Is.EqualTo ("StorageProvider"));
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
    }

    [Test]
    public void GetToString()
    {
      ClassDefinition actual = new ReflectionBasedClassDefinition ("OrderID", "OrderTable", "StorageProvider", typeof (Order), false, new List<Type>());

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ReflectionBasedClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType()
    {
      ReflectionBasedClassDefinition actual = new ReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order), false, new List<Type>());

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType()
    {
      ReflectionBasedClassDefinition actual = new ReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (AbstractClass), true, new List<Type>());

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentFalse()
    {
      ReflectionBasedClassDefinition actual = 
          new ReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (AbstractClass), false, new List<Type>());

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentTrue()
    {
      ReflectionBasedClassDefinition actual = new ReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (Order), true, new List<Type>());

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetRelationDefinition()
    {
      RelationDefinition relation = _orderClass.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", relation.ID);
    }

    [Test]
    public void GetUndefinedRelationDefinition()
    {
      Assert.IsNull (_orderClass.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void GetAllRelationDefinitions()
    {
      RelationDefinitionCollection relations = _distributorClass.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (5, relations.Count);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"]);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"]);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor"]);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company"]);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyRelationDefinition()
    {
      _orderClass.GetRelationDefinition (string.Empty);
    }

    [Test]
    public void GetRelationDefinitionWithInheritance()
    {
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    public void GetRelatedClassDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelatedClassDefinitionWithEmtpyPropertyName()
    {
      _distributorClass.GetOppositeClassDefinition (string.Empty);
    }

    [Test]
    public void GetRelationEndPointDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelationEndPointDefinitionFromEmptyPropertyName()
    {
      _orderClass.GetRelationEndPointDefinition (string.Empty);
    }

    [Test]
    public void IsRelationEndPointTrue()
    {
      RelationDefinition orderToOrderItem =
          TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"];
      IRelationEndPointDefinition endPointDefinition =
          orderToOrderItem.GetEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      Assert.IsTrue (_orderClass.IsRelationEndPoint (endPointDefinition));
    }

    [Test]
    public void IsRelationEndPointFalse()
    {
      RelationDefinition partnerToPerson =
          TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition ("Partner", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson");

      Assert.IsFalse (_orderClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsRelationEndPointWithNull()
    {
      _orderClass.IsRelationEndPoint (null);
    }

    [Test]
    public void IsRelationEndPointWithInheritance()
    {
      RelationDefinition partnerToPerson =
          TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition ("Partner", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson");

      Assert.IsTrue (_distributorClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    public void GetPropertyDefinition()
    {
      Assert.IsNotNull (_orderClass.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyPropertyDefinition()
    {
      _orderClass.GetPropertyDefinition (string.Empty);
    }

    [Test]
    public void GetInheritedPropertyDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Type 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassNotDerivedFromDomainObject' of class 'Company' is not derived from "
        + "'Rubicon.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeWithInvalidDerivation()
    {
      new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (ClassNotDerivedFromDomainObject), false, new List<Type>());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Type 'Rubicon.Data.DomainObjects.DomainObject' of class 'Company' is not derived from 'Rubicon.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeDomainObject()
    {
      new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (DomainObject), false, new List<Type>());
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void BaseClassWithDifferentStorageProvider()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "Provider 1", typeof (Company), false, new List<Type>());
      new ReflectionBasedClassDefinition ("Customer", "Company", "Provider 2", typeof (Customer), false, companyClass, new List<Type>());
    }

    [Test]
    public void ClassTypeIsNotDerivedFromBaseClassType()
    {
      ReflectionBasedClassDefinition orderClass = new ReflectionBasedClassDefinition ("Order", "Order",c_testDomainProviderID, typeof (Order), false, new List<Type>());

      try
      {
        new ReflectionBasedClassDefinition ("Distributor", "Company",c_testDomainProviderID, typeof (Distributor), false, orderClass, new List<Type>());
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.",
            typeof (Distributor).AssemblyQualifiedName,
            "Distributor",
            orderClass.ClassType.AssemblyQualifiedName,
            orderClass.ID);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Proeprty 'Name' cannot be added to class 'Company', because it already defines a property with the same name.")]
    public void AddDuplicateProperty()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Proeprty 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());
      ReflectionBasedClassDefinition orderClass = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type>());

      ReflectionBasedPropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100);
      orderClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Proeprty 'Name' cannot be added to class 'Company', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClassWithSameID ()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());
      ReflectionBasedClassDefinition otherCompanyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      ReflectionBasedPropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100);
      otherCompanyClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Proeprty 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          "Customer", "Company", "TestDomain", typeof (Customer), false, companyClass, new List<Type>());
      customerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(customerClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Proeprty 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      ReflectionBasedClassDefinition partnerClass = new ReflectionBasedClassDefinition (
          "Partner", "Company", "TestDomain", typeof (Partner), false, companyClass, new List<Type>());

      ReflectionBasedClassDefinition supplierClass = new ReflectionBasedClassDefinition (
          "Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass, new List<Type>());

      supplierClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(supplierClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Class 'Partner' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void ValidateMappingWithDuplicatePropertyBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      ReflectionBasedClassDefinition partnerClass = new ReflectionBasedClassDefinition (
          "Partner", "Company", "TestDomain", typeof (Partner), false, companyClass, new List<Type>());

      partnerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(partnerClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      companyClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Class 'Supplier' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void ValidateMappingWithDuplicatePropertyBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      ReflectionBasedClassDefinition partnerClass = new ReflectionBasedClassDefinition (
          "Partner", "Company", "TestDomain", typeof (Partner), false, companyClass, new List<Type>());

      ReflectionBasedClassDefinition supplierClass = new ReflectionBasedClassDefinition (
          "Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass, new List<Type>());

      supplierClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(supplierClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      companyClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    public void ConstructorWithoutBaseClass()
    {
      new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "No relation found for class 'Order' and property 'UndefinedProperty'.")]
    public void GetMandatoryRelationEndPointDefinitionForUndefinedProperty()
    {
      _orderClass.GetMandatoryRelationEndPointDefinition ("UndefinedProperty");
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition =
          _orderClass.GetMandatoryOppositeEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      Assert.IsNotNull (oppositeEndPointDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition customerEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
      IRelationEndPointDefinition orderTicketEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      IRelationEndPointDefinition officialEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official");

      Assert.AreEqual (4, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, customerEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderTicketEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderItemsEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, officialEndPoint) >= 0);
    }

    [Test]
    public void GetRelationEndPointDefinitions()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetMyRelationEndPointDefinitions();

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (1, relationEndPointDefinitions.Length);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn", relationEndPointDefinitions[0].PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition classWithoutRelatedClassIDColumnEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn");
      IRelationEndPointDefinition contactPersonEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson");
      IRelationEndPointDefinition ceoEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");
      IRelationEndPointDefinition classWithoutRelatedClassIDColumnAndDerivationEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation");
      IRelationEndPointDefinition industrialSectorEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector");

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (5, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, contactPersonEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, ceoEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnAndDerivationEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, industrialSectorEndPoint) >= 0);
    }

    [Test]
    public void GetDerivedClassesWithoutInheritance()
    {
      Assert.IsNotNull (_orderClass.DerivedClasses);
      Assert.AreEqual (0, _orderClass.DerivedClasses.Count);
      Assert.IsTrue (_orderClass.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void GetDerivedClassesWithInheritance()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Partner"));
      Assert.IsTrue (companyDefinition.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void GetRelationDefinitions()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinitionCollection clientRelations = clientDefinition.GetRelationDefinitions();

      Assert.AreEqual (1, clientRelations.Count);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", clientRelations[0].ID);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinition parentClient =
          MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"];
      AnonymousRelationEndPointDefinition clientAnonymousEndPointDefinition =
          (AnonymousRelationEndPointDefinition) parentClient.GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition folderEndPoint =
          folderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint =
          folderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder");

      Assert.IsTrue (folderDefinition.IsMyRelationEndPoint (folderEndPoint));
      Assert.IsFalse (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (2, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
      Assert.IsTrue (Contains (endPointDefinitions, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      RelationDefinitionCollection relations = fileSystemItemDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"]);
    }

    [Test]
    public void GetRelationDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      RelationDefinitionCollection relations = folderDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"]);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.AreSame (
          folderDefinition,
          folderDefinition.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.AreSame (
          fileSystemItemDefinition,
          folderDefinition.GetOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeEndPointDefinitionWithInvalidPropertyName()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryRelationDefinition()
    {
      RelationDefinition relation = _orderClass.GetMandatoryRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", relation.ID);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'Order' and property 'InvalidProperty'.")]
    public void GetMandatoryRelationDefinitionWithInvalidPropertyName()
    {
      _orderClass.GetMandatoryRelationDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryPropertyDefinition()
    {
      Assert.IsNotNull (_orderClass.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Order' does not contain the property 'InvalidProperty'.")]
    public void GetMandatoryPropertyDefinitionWithInvalidPropertName()
    {
      _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
    }

    [Test]
    public void SetClassDefinitionOfPropertyDefinition()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type>());
     
      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Test", "Test", typeof (int));
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void CancelAddingOfPropertyDefinition()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type>());

      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Test", "Test", typeof (int));
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      new PropertyDefinitionCollectionEventReceiver (classDefinition.MyPropertyDefinitions, true);
      try
      {
        classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
        Assert.Fail ("Expected an EventReceiverCancelException.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
      }
    }

    [Test]
    public void Contains()
    {
      Assert.IsFalse (_orderClass.Contains (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_orderClass, "PropertyName", "ColumnName", typeof (int))));
      Assert.IsTrue (_orderClass.Contains (_orderClass["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void PropertyDefinitionCollectionBackLink()
    {
      Assert.AreSame (_orderClass, _orderClass.MyPropertyDefinitions.ClassDefinition);
    }

    [Test]
    public void GetInheritanceRootClass()
    {
      ClassDefinition expected = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass());
    }

    [Test]
    public void GetAllDerivedClasses()
    {
      ClassDefinition companyClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      ClassDefinitionCollection allDerivedClasses = companyClass.GetAllDerivedClasses();
      Assert.IsNotNull (allDerivedClasses);
      Assert.AreEqual (4, allDerivedClasses.Count);

      Assert.IsTrue (allDerivedClasses.Contains (typeof (Customer)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Partner)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Supplier)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Distributor)));
    }

    [Test]
    public void IsSameOrBaseClassOfFalse()
    {
      Assert.IsFalse (_orderClass.IsSameOrBaseClassOf (_distributorClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithSameClass()
    {
      Assert.IsTrue (_orderClass.IsSameOrBaseClassOf (_orderClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithBaseClass()
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

    [Test]
    public void PropertyInfoWithSimpleProperty ()
    {
      PropertyInfo property = typeof (Order).GetProperty ("OrderNumber");
      ReflectionBasedPropertyDefinition propertyDefinition =
          (ReflectionBasedPropertyDefinition) _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.AreEqual (property, propertyDefinition.PropertyInfo);
    }

    [Instantiable]
    [DBTable]
    public abstract class Base : DomainObject
    {
      public int Name
      {
        get { return Properties[typeof (Base), "Name"].GetValue<int> (); }
      }
    }

    [Instantiable]
    public abstract class Shadower : Base
    {
      [DBColumn ("NewName")]
      public new int Name
      {
        get { return Properties[typeof (Shadower), "Name"].GetValue<int> (); }
      }
    }

    [Test]
    public void PropertyInfoWithShadowedProperty ()
    {
      PropertyInfo property1 = typeof (Shadower).GetProperty ("Name",
          BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      PropertyInfo property2 =
          typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      ClassDefinition classDefinition1 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Shadower));
      ClassDefinition classDefinition2 = classDefinition1.BaseClass;

      ReflectionBasedPropertyDefinition propertyDefinition1 =
          (ReflectionBasedPropertyDefinition) classDefinition1.GetMandatoryPropertyDefinition (property1.DeclaringType.FullName + "." + property1.Name);
      ReflectionBasedPropertyDefinition propertyDefinition2 =
          (ReflectionBasedPropertyDefinition) classDefinition2.GetMandatoryPropertyDefinition (property2.DeclaringType.FullName + "." + property2.Name);
      
      Assert.AreEqual (property1, propertyDefinition1.PropertyInfo);
      Assert.AreEqual (property2, propertyDefinition2.PropertyInfo);
    }

    [Test]
    public void CreatorIsFactoryBasedCreator ()
    {
      object creatorInstance = PrivateInvoke.GetPublicStaticField (
          typeof (ReflectionBasedClassDefinition).Assembly.GetType ("Rubicon.Data.DomainObjects.Infrastructure.FactoryBasedDomainObjectCreator", true),
          "Instance");
      Assert.AreEqual (creatorInstance, PrivateInvoke.InvokeNonPublicMethod (_orderClass, "GetDomainObjectCreator"));
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoPersistentChanges ()
    {
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinA)))
      {
        classDefinition.ValidateCurrentMixinConfiguration (); // ok, no changes
      }

      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (NonDomainObjectMixin), typeof (MixinA)))
      {
        classDefinition.ValidateCurrentMixinConfiguration (); // ok, no persistence-related changes
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
        + "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order after the mapping information was built: "
        + "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain.MixinA.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixisMissing ()
    {
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
      using (MixinConfiguration.ScopedExtend (typeof (Order)))
      {
        classDefinition.ValidateCurrentMixinConfiguration ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "One or more persistence-related mixins were added to the domain object type "
        + "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order after the mapping information was built: "
        + "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain.MixinB, "
        + "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain.MixinC.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)))
      {
        classDefinition.ValidateCurrentMixinConfiguration ();
        Assert.Fail ();
      }
    }
  }
}