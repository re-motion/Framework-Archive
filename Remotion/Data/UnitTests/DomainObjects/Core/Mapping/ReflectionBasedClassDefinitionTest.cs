// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Client = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client;
using Customer = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer;
using FileSystemItem = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem;
using Folder = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder;
using Order = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order;
using Person = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _orderClass;
    private ReflectionBasedClassDefinition _distributorClass;

    private ReflectionBasedClassDefinition _targetClassForPersistentMixinClass;
    private ReflectionBasedClassDefinition _derivedTargetClassForPersistentMixinClass;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private ReflectionBasedClassDefinition _domainBaseClass;
    private ReflectionBasedClassDefinition _personClass;
    private ReflectionBasedClassDefinition _customerClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));

      _domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase", null, UnitTestDomainStorageProviderDefinition, typeof (DomainBase), false);
      _personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false, _domainBaseClass);
      _customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", null, UnitTestDomainStorageProviderDefinition, typeof (Customer), false, _personClass);
      _organizationalUnitClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrganizationalUnit",
          "TableInheritance_OrganizationalUnit",
          UnitTestDomainStorageProviderDefinition,
          typeof (OrganizationalUnit),
          false,
          _domainBaseClass);

      _domainBaseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _personClass, _organizationalUnitClass }, true, true));

      _orderClass = (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _distributorClass = (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];

      _targetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (DerivedTargetClassForPersistentMixin)];
    }

    [Test]
    public void Initialize ()
    {
      var actual = new ReflectionBasedClassDefinition ("Order", typeof (Order), false, null, null, new PersistentMixinFinder (typeof (Order)));
      actual.SetDerivedClasses (new ClassDefinitionCollection (true));

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.StorageEntityDefinition, Is.Null);
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
      Assert.That (actual.IsReadOnly, Is.False);
    }

    [Test]
    public void InitializeWithNullStorageGroupType ()
    {
      Assert.That (_domainBaseClass.StorageGroupType, Is.Null);

      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DomainBase),
          false,
          null,
          null,
          new PersistentMixinFinder (typeof (DomainBase)));
      Assert.That (classDefinition.StorageGroupType, Is.Null);
    }

    [Test]
    public void InitializeWithStorageGroupType ()
    {
      Assert.That (_domainBaseClass.StorageGroupType, Is.Null);

      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DomainBase),
          false,
          null,
          typeof (DBStorageGroupAttribute),
          new PersistentMixinFinder (typeof (DomainBase)));
      Assert.That (classDefinition.StorageGroupType, Is.Not.Null);
      Assert.That (classDefinition.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void NullEntityNameWithDerivedClass ()
    {
      Assert.IsNull (StorageModelTestHelper.GetEntityName (_domainBaseClass));
      Assert.IsNotNull (StorageModelTestHelper.GetEntityName (_personClass));
      Assert.IsNull (StorageModelTestHelper.GetEntityName (_customerClass));
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName());
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingle ()
    {
      string[] entityNames = _customerClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void SetStorageEntityDefinition ()
    {
      var tableDefinition = new TableDefinition (_storageProviderDefinition, "Tablename", "Viewname", new ColumnDefinition[0]);

      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetStorageEntityDefinition_ClassIsReadOnly ()
    {
      var tableDefinition = new TableDefinition (_storageProviderDefinition, "Tablename", "Viewname", new ColumnDefinition[0]);
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_domainBaseClass, "Test", "Test");

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));

      Assert.That (_domainBaseClass.MyPropertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyPropertyDefinitions[0], Is.SameAs (propertyDefinition));
      Assert.That (_domainBaseClass.MyPropertyDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The property-definitions for class 'DomainBase' have already been set."
        )]
    public void SetPropertyDefinitions_Twice_ThrowsException ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_domainBaseClass, "Test", "Test");

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetPropertyDefinitions_ClassIsReadOnly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
    }

    [Test]
    public void SetRelationEndPointDefinitions ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (_domainBaseClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));

      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions[0], Is.SameAs (endPointDefinition));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "Relation end point for property 'Test' cannot be added to class 'DomainBase', because it was initialized for class 'Distributor'.")]
    public void SetRelationEndPointDefinitions_DifferentClassDefinition_ThrowsException ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (_distributorClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      
      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
      "The relation end point definitions for class 'DomainBase' have already been set.")]
    public void SetRelationEndPointDefinitions_Twice_ThrowsException ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (_domainBaseClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetRelationEndPointDefinitions_ClassIsReadonly ()
    {
      _domainBaseClass.SetReadOnly ();

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[0], true));
    }

    [Test]
    public void SetDerivedClasses ()
    {
      _personClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _customerClass }, false, true));

      Assert.That (_personClass.DerivedClasses.Count, Is.EqualTo (1));
      Assert.That (_personClass.DerivedClasses[0], Is.SameAs (_customerClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The derived-classes for class 'Person' have already been set.")]
    public void SetDerivedClasses_Twice_ThrowsException ()
    {
      _personClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _customerClass }, false, true));
      _personClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _customerClass }, false, true));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'Person' is read-only.")]
    public void SetDerivedClasses_ClasssIsReadOnly ()
    {
      _personClass.SetReadOnly();
      _personClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _orderClass }, false, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Derived class 'Order' cannot be added to class 'Person', because it has no base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasNoBaseClassDefined ()
    {
      _personClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _orderClass }, false, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Derived class 'Person' cannot be added to class 'Customer', because it has class 'DomainBase' as its base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasWrongBaseClassDefined ()
    {
      _customerClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _personClass }, false, true));
    }

    [Test]
    public void GetAllConcreteEntityNamesForConcrete ()
    {
      string[] entityNames = _personClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingleWithEntityName ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, personClass);

      string[] entityNames = customerClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClass ()
    {
      // ensure both classes derived from DomainBase are loaded
      Dev.Null = _personClass;
      Dev.Null = _organizationalUnitClass;

      string[] entityNames = _domainBaseClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (2, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
      Assert.AreEqual ("TableInheritance_OrganizationalUnit", entityNames[1]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClassWithSameEntityNameInInheritanceHierarchy ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase", null, UnitTestDomainStorageProviderDefinition, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false, domainBaseClass);
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, personClass);

      domainBaseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { personClass }, true, true));

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void SetReadOnly ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      actual.SetDerivedClasses (new ClassDefinitionCollection (true));
      Assert.That (actual.IsReadOnly, Is.False);

      actual.SetReadOnly();

      Assert.That (actual.IsReadOnly, Is.True);
      Assert.That (actual.DerivedClasses.IsReadOnly, Is.True);
    }

    [Test]
    public void GetToString ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderID", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ReflectionBasedClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (AbstractClass), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentFalse ()
    {
      ReflectionBasedClassDefinition actual =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassID", "Table", UnitTestDomainStorageProviderDefinition, typeof (AbstractClass), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentTrue ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassID", "Table", UnitTestDomainStorageProviderDefinition, typeof (Order), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetRelatedClassDefinition ()
    {
      Assert.IsNotNull (
          _distributorClass.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"));
      Assert.IsNotNull (
          _distributorClass.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
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
      Assert.IsNotNull (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"));
      Assert.IsNotNull (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
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
      RelationDefinition orderToOrderItem =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"];
      IRelationEndPointDefinition endPointDefinition =
          orderToOrderItem.GetEndPointDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems");

      Assert.IsTrue (_orderClass.IsRelationEndPoint (endPointDefinition));
    }

    [Test]
    public void IsRelationEndPointFalse ()
    {
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              +
              "TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");

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
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner:"
              +
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");

      Assert.IsTrue (_distributorClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      Assert.IsNotNull (
          _orderClass.GetPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetPropertyDefinition_NoPropertyDefinitionsHaveBeenSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Order));
      classDefinition.GetPropertyDefinition ("dummy");
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
      Assert.IsNotNull (
          _distributorClass.GetPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
    }

    [Test]
    public void GetAllPropertyDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [ExpectedException (typeof (InvalidOperationException), 
      ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetAllPropertyDefinitions_ThrowsWhenPropertiesNotSet ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order",
          "OrderTable",
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub)),
          typeof (Order),
          false);
      classDefinition.GetPropertyDefinitions();
    }

    [Test]
    public void GetAllPropertyDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Test", "Test");
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetPropertyDefinitions();
      var result2 = classDefinition.GetPropertyDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllPropertyDefinitions_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var orderClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      orderClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var companyPropertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "Company", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, companyClass);
      var customerPropertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (customerClass, "Name", "Name");
      customerClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { customerPropertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var companyPropertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Partner", "Company", UnitTestDomainStorageProviderDefinition, typeof (Partner), false, companyClass);
      partnerClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      var supplierClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Supplier", "Company", UnitTestDomainStorageProviderDefinition, typeof (Supplier), false, partnerClass);
      var supplierPropertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (supplierClass, "Name", "Name");
      supplierClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { supplierPropertyDefinition }, true));
    }

    [Test]
    public void ConstructorWithoutBaseClass ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);

      // Expectation: no exception
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
      IRelationEndPointDefinition oppositeEndPointDefinition =
          _orderClass.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket");
      Assert.IsNotNull (oppositeEndPointDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order",
          oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[]{ endPointDefinition}, true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
      ExpectedMessage = "No relation end point definitions have been set for class 'Order'.")]
    public void GetAllRelationEndPointDefinitions_ThrowsWhenRelationsNotSet ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.GetRelationEndPointDefinitions();
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition(classDefinition, "Test", false, CardinalityType.One, typeof(DomainObject), null, typeof(Order).GetProperty("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[]{endPointDefinition}, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetRelationEndPointDefinitions();
      var result2 = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllRelationEndPointDefinitionss_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[]{endPointDefinition}, true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Content ()
    {
      var relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition customerEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer");
      IRelationEndPointDefinition orderTicketEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems");
      IRelationEndPointDefinition officialEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official");

      Assert.That (
          relationEndPointDefinitions, Is.EquivalentTo (new[] { customerEndPoint, orderTicketEndPoint, orderItemsEndPoint, officialEndPoint }));
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.MyRelationEndPointDefinitions.ToArray();

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (1, relationEndPointDefinitions.Length);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn",
          relationEndPointDefinitions[0].PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance ()
    {
      var relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition classWithoutRelatedClassIDColumnEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn");
      IRelationEndPointDefinition contactPersonEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");
      IRelationEndPointDefinition ceoEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo");
      IRelationEndPointDefinition classWithoutRelatedClassIDColumnAndDerivationEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation");
      IRelationEndPointDefinition industrialSectorEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector");

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.That (
          relationEndPointDefinitions,
          Is.EquivalentTo (
              new[]
              {
                  classWithoutRelatedClassIDColumnEndPoint,
                  contactPersonEndPoint,
                  ceoEndPoint,
                  classWithoutRelatedClassIDColumnAndDerivationEndPoint,
                  industrialSectorEndPoint
              }));
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
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Partner"));
      Assert.IsTrue (companyDefinition.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinition parentClient =
          MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Client.ParentClient"];
      var clientAnonymousEndPointDefinition = (AnonymousRelationEndPointDefinition) parentClient.GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient"));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition folderEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder");

      Assert.IsTrue (folderDefinition.IsMyRelationEndPoint (folderEndPoint));
      Assert.IsFalse (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      var endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Count());
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      var endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (2, endPointDefinitions.Count());
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.AreSame (
          folderDefinition,
          folderDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.AreSame (
          fileSystemItemDefinition,
          folderDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'Invalid'.")]
    public void GetMandatoryOppositeEndPointDefinition_InvalidProperty ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("Invalid");
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeEndPointDefinition_InvalidProperty ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNull (fileSystemItemDefinition.GetOppositeEndPointDefinition ("Invalid"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeEndPointDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetMandatoryOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryPropertyDefinition ()
    {
      Assert.IsNotNull (
          _orderClass.GetMandatoryPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
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
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "Test", "Test");
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void Contains ()
    {
      Assert.IsFalse (
          _orderClass.Contains (
              ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
                  _orderClass, "PropertyName", "ColumnName")));
      Assert.IsTrue (
          _orderClass.Contains (
              _orderClass["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
    }

    [Test]
    public void GetInheritanceRootClass ()
    {
      ClassDefinition expected = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass());
    }

    [Test]
    public void GetAllDerivedClasses ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      ClassDefinitionCollection allDerivedClasses = companyClass.GetAllDerivedClasses();
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
      ClassDefinition companyClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      Assert.IsTrue (companyClass.IsSameOrBaseClassOf (_distributorClass));
    }

    private bool Contains (IEnumerable<IRelationEndPointDefinition> endPointDefinitions, string propertyName)
    {
      return endPointDefinitions.Any (endPointDefinition => endPointDefinition.PropertyName == propertyName);
    }

    [Test]
    public void PropertyInfoWithSimpleProperty ()
    {
      PropertyInfo property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition =
          (ReflectionBasedPropertyDefinition) _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.AreEqual (property, propertyDefinition.PropertyInfo);
    }



  

    [Test]
    public void CreatorIsFactoryBasedCreator ()
    {
      Assert.AreEqual (InterceptedDomainObjectCreator.Instance, _orderClass.GetDomainObjectCreator());
    }

    [Test]
    public void PersistentMixinFinder ()
    {
      var mixinFinder = new PersistentMixinFinderMock (typeof (Order));
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixinFinder);
      Assert.That (classDefinition.PersistentMixinFinder, Is.SameAs (mixinFinder));
    }

    [Test]
    public void PersistentMixins_Empty ()
    {
      var mixins = new Type[0];
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }

    [Test]
    public void PersistentMixins_NonEmpty ()
    {
      var mixins = new[] { typeof (MixinA), typeof (MixinB) };
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }


    [Test]
    public void ResolveProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_Twice_ReturnsSamePropertyDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result1 = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));
      var result2 = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveProperty_StorageClassNoneProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveProperty_MixinProperty ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _targetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_MixinPropertyOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToOne ()
    {
      var property = typeof (Order).GetProperty ("OrderTicket");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToMany ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_NonEndPointProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveRelationEndPoint_Twice_ReturnsSameRelationDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result1 = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));
      var result2 = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_VirtualEndPoint ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");

      var result = _targetClassForPersistentMixinClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_DefinedOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }
  }
}