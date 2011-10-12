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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointNamesAreConsistentValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest : MappingReflectionTestBase
  {
    private ClassDefinition[] _emptyClassDefinitions;
    private RelationDefinition[] _emptyRelationDefinitions;

    private MockRepository _mockRepository;
    private IMappingLoader _mockMappingLoader;
    private ReflectionBasedNameResolver _nameResolver;
    private TableDefinition _fakeStorageEntityDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _emptyClassDefinitions = new ClassDefinition[0];
      _emptyRelationDefinitions = new RelationDefinition[0];

      _nameResolver = new ReflectionBasedNameResolver();
      _mockRepository = new MockRepository();
      _mockMappingLoader = _mockRepository.StrictMock<IMappingLoader>();

      _fakeStorageEntityDefinition = TableDefinitionObjectMother.Create (
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, new EntityNameDefinition (null, "Test"));
    }

    [Test]
    public void Initialize ()
    {
      var typeDefinitions = new[]
                             {
                                 new ClassDefinition ("CID1", typeof (Ceo), false, null, null, MockRepository.GenerateStub<IPersistentMixinFinder>()),
                                 new ClassDefinition (
                                     "CID2", typeof (Order), false, null, null, MockRepository.GenerateStub<IPersistentMixinFinder>())
                             };
      foreach (var typeDefinition in typeDefinitions)
      {
        typeDefinition.SetDerivedClasses (new ClassDefinition[0]);
        typeDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
        typeDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      }

      var relationDefinitions = new[]
                                {
                                    new RelationDefinition (
                                        "RID1",
                                        MockRepository.GenerateStub<IRelationEndPointDefinition>(),
                                        MockRepository.GenerateStub<IRelationEndPointDefinition>()),
                                    new RelationDefinition (
                                        "RID2",
                                        MockRepository.GenerateStub<IRelationEndPointDefinition>(),
                                        MockRepository.GenerateStub<IRelationEndPointDefinition>())
                                };

      StubMockMappingLoader (typeDefinitions, relationDefinitions);

      _mockRepository.ReplayAll();

      var configuration = new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      _mockRepository.VerifyAll();

      Assert.That (configuration.ResolveTypes, Is.True);
      Assert.That (configuration.NameResolver, Is.SameAs (_nameResolver));
    }

    [Test]
    public void SetCurrent ()
    {
      try
      {
        StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);

        _mockRepository.ReplayAll();

        var configuration = new MappingConfiguration (
            _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
        MappingConfiguration.SetCurrent (configuration);

        Assert.AreSame (configuration, MappingConfiguration.Current);
      }
      finally
      {
        MappingConfiguration.SetCurrent (null);
      }
    }

    [Test]
    public void GetTypeDefinitions ()
    {
      var classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition1.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      classDefinition1.SetDerivedClasses (Enumerable.Empty<ClassDefinition> ());

      var classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass1));
      classDefinition2.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      classDefinition2.SetDerivedClasses (Enumerable.Empty<ClassDefinition> ());

      StubMockMappingLoader (new[] { classDefinition1, classDefinition2 }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.GetTypeDefinitions(), Is.EquivalentTo (new[] { classDefinition1, classDefinition2 }));
    }

    [Test]
    public void ContainsTypeDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      classDefinition.SetDerivedClasses (Enumerable.Empty<ClassDefinition> ());
      StubMockMappingLoader (new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.ContainsTypeDefinition (typeof (RelationEndPointPropertyClass)), Is.True);
    }

    [Test]
    public void ContainsTypeDefinition_ValueNotFound ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.ContainsTypeDefinition (typeof (RelationEndPointPropertyClass)), Is.False);
    }

    [Test]
    public void GetTypeDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());
      StubMockMappingLoader (new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.GetTypeDefinition (typeof (RelationEndPointPropertyClass)), Is.SameAs (classDefinition));
    }

    [Test]
    public void GetTypeDefinition_ValueNotFound ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (
          () => configuration.GetTypeDefinition (typeof (DomainObject)),
          Throws.Exception.TypeOf<MappingException>()
              .And.Message.EqualTo (string.Format ("Mapping does not contain class '{0}'.", typeof (DomainObject))));
    }

    [Test]
    public void GetTypeDefinition_ValueNotFound_CustomException ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (
          () => configuration.GetTypeDefinition (typeof (DomainObject), t =>new ApplicationException (t.Name)),
          Throws.Exception.TypeOf<ApplicationException> ()
              .And.Message.EqualTo (typeof (DomainObject).Name));
    }

    [Test]
    public void ContainsClassDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      classDefinition.SetDerivedClasses (Enumerable.Empty<ClassDefinition> ());
      StubMockMappingLoader (new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.ContainsClassDefinition (classDefinition.ID), Is.True);
    }

    [Test]
    public void ContainsClassDefinition_ValueNotFound ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.ContainsClassDefinition ("ID"), Is.False);
    }

    [Test]
    public void GetClassDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      classDefinition.SetDerivedClasses (Enumerable.Empty<ClassDefinition> ());
      StubMockMappingLoader (new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (configuration.GetClassDefinition (classDefinition.ID), Is.SameAs (classDefinition));
    }

    [Test]
    public void GetClassDefinition_ValueNotFound ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (
          () => configuration.GetClassDefinition ("ID"),
          Throws.Exception.TypeOf<MappingException> ()
              .And.Message.EqualTo ("Mapping does not contain class 'ID'."));
    }

    [Test]
    public void GetClassDefinition_ValueNotFound_CustomException ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub ();
      _mockRepository.ReplayAll ();
      var configuration = new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);

      Assert.That (
          () => configuration.GetClassDefinition ("ID", id =>new ApplicationException (id)),
          Throws.Exception.TypeOf<ApplicationException> ()
              .And.Message.EqualTo ("ID"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Generic domain objects are not supported.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]")]
    public void ClassDefinitionsAreValidated ()
    {
      var type = typeof (GenericTypeDomainObject<string>);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (type);

      StubMockMappingLoaderWithValidation (new[] { classDefinition }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only StorageClass.Persistent and StorageClass.Transaction are supported for property 'PropertyWithStorageClassNone' of class "
        + "'DerivedValidationDomainObjectClass'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        + "Property: PropertyWithStorageClassNone")]
    public void PropertyDefinitionsAreValidated ()
    {
      var type = typeof (DerivedValidationDomainObjectClass);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (
          type.Name, type.Name, TestDomainStorageProviderDefinition, type, false);
      var propertyInfo = type.GetProperty ("PropertyWithStorageClassNone");
      var propertyDefinition = PropertyDefinitionFactory.Create(classDefinition, StorageClass.None, propertyInfo);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      StubMockMappingLoaderWithValidation (new[] { classDefinition }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer\r\n"
        + "Property: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders")]
    public void RelationDefinitionsAreValidated ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      var relationDefinition =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"];

      StubMockMappingLoaderWithValidation (new[] { classDefinition }, new[] { relationDefinition });
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
        + "Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of its base classes.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass")]
    public void PersistenceModelIsValidated ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (TestDomainStorageProviderDefinition);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false);
      classDefinition.SetStorageEntity (unionViewDefinition);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      StubMockMappingLoaderWithValidation (new[] { classDefinition }, new RelationDefinition[0]);

      var persistenceModelLoaderStub = _mockRepository.Stub<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Stub (stub => stub.ApplyPersistenceModelToHierarchy (Arg<ClassDefinition>.Is.Anything));
      persistenceModelLoaderStub
          .Stub (stub => stub.CreatePersistenceMappingValidator (Arg<ClassDefinition>.Is.Anything))
          .Return (new PersistenceMappingValidator (new ClassAboveTableIsAbstractValidationRule()));

      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllRootClasses ()
    {
      var persistenceModelLoaderMock = MockRepository.GenerateStrictMock<IPersistenceModelLoader>();
      var validatorMock1 = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();
      var validatorMock2 = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();

      var rootClass1 = ClassDefinitionObjectMother.CreateFinishedClassDefinition (typeof (Order));
      var rootClass2 = ClassDefinitionObjectMother.CreateFinishedClassDefinition (typeof (OrderTicket));

      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass1))
          .WhenCalled (mi => rootClass1.SetStorageEntity (_fakeStorageEntityDefinition));
      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass2))
          .WhenCalled (mi => rootClass2.SetStorageEntity (_fakeStorageEntityDefinition));

      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass1)).Return (validatorMock1);
      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass2)).Return (validatorMock2);
      persistenceModelLoaderMock.Replay();

      validatorMock1
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass1 })))
          .Return (new MappingValidationResult[0]);
      validatorMock2
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass2 })))
          .Return (new MappingValidationResult[0]);
      validatorMock1.Replay();
      validatorMock2.Replay();

      StubMockMappingLoaderWithValidation (new[] { rootClass1, rootClass2 }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderMock);

      persistenceModelLoaderMock.VerifyAllExpectations();
      validatorMock1.VerifyAllExpectations();
      validatorMock2.VerifyAllExpectations();
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllDerivedClasses ()
    {
      var persistenceModelLoaderMock = MockRepository.GenerateStrictMock<IPersistenceModelLoader>();
      var validatorMock = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();

      var rootClass = ClassDefinitionObjectMother.CreateFinishedFileSystemItemDefinitionWithDerivedClasses();
      var derivedClass1 = rootClass.DerivedClasses[0];
      var derivedClass2 = rootClass.DerivedClasses[1];

      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass)).WhenCalled (
              mi =>
              {
                rootClass.SetStorageEntity (_fakeStorageEntityDefinition);
                derivedClass1.SetStorageEntity (_fakeStorageEntityDefinition);
                derivedClass2.SetStorageEntity (_fakeStorageEntityDefinition);
              });
      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass)).Return (validatorMock);
      persistenceModelLoaderMock.Replay();

      validatorMock
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass, derivedClass1, derivedClass2 })))
          .Return (new MappingValidationResult[0]);
      validatorMock.Replay();

      StubMockMappingLoaderWithValidation (new[] { rootClass, derivedClass1, derivedClass2 }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderMock);

      persistenceModelLoaderMock.VerifyAllExpectations();
      validatorMock.VerifyAllExpectations();
    }

    [Test]
    public void PersistenceModelIsLoaded ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var propertyDefinition1 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition,
          "OrderNumber",
          "FakeColumn1",
          typeof (int),
          false,
          null,
          StorageClass.Persistent);
      propertyDefinition1.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("FakeColumn1"));
      var propertyDefinition2 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition,
          "DeliveryDate",
          "FakeColumn2",
          typeof (DateTime),
          false,
          null,
          StorageClass.Persistent);
      propertyDefinition2.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("FakeColumn2"));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      StubMockMappingLoader (new[] { classDefinition }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      Assert.That (classDefinition.StorageEntityDefinition, Is.Not.Null);
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName.EntityName, Is.EqualTo ("Order"));
      Assert.That (classDefinition.MyPropertyDefinitions.Count, Is.EqualTo (2));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetAllColumns().Count(), Is.EqualTo (5));

      var storageProperty1 = ((SimpleStoragePropertyDefinition) classDefinition.MyPropertyDefinitions["OrderNumber"].StoragePropertyDefinition);
      Assert.That (storageProperty1, Is.Not.Null);
      Assert.That (storageProperty1.ColumnDefinition.Name, Is.EqualTo ("FakeColumn1"));

      var storageProperty2 = (SimpleStoragePropertyDefinition) classDefinition.MyPropertyDefinitions["DeliveryDate"].StoragePropertyDefinition;
      Assert.That (storageProperty2, Is.Not.Null);
      Assert.That (storageProperty2.ColumnDefinition.Name, Is.EqualTo ("FakeColumn2"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage entity to class 'Order'.")]
    public void VerifyPersistenceModelApplied_NoStorageEntityIsAppliedToTheRootClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (new[] { classDefinition }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage property to property 'Fake' of class 'Order'.")]
    public void VerifyPersistenceModelApplied_NoStoragePropertyIsAppliedToTheRootClassProperty ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Fake", "Fake");
      PrivateInvoke.SetNonPublicField (propertyDefinition, "_storagePropertyDefinition", null);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (propertyDefinition.StoragePropertyDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (new[] { classDefinition }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      persistenceModelStub.Stub (stub => stub.ApplyPersistenceModelToHierarchy (classDefinition)).WhenCalled (
          mi =>
          classDefinition.SetStorageEntity (fakeStorageEntityDefinition));

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage entity to class 'Partner'.")]
    public void VerifyPersistenceModelApplied_NoStorageEntityIsAppliedToDerivedClass ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Company), null);
      var partnerClass = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Partner), companyClass);

      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      partnerClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      companyClass.SetDerivedClasses (new[] { partnerClass });

      Assert.That (companyClass.StorageEntityDefinition, Is.Null);
      Assert.That (partnerClass.StorageEntityDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (new[] { companyClass }, new RelationDefinition[0]);
      _mockRepository.ReplayAll();

      persistenceModelStub.Stub (stub => stub.ApplyPersistenceModelToHierarchy (companyClass)).WhenCalled (
          mi => companyClass.SetStorageEntity (fakeStorageEntityDefinition));

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes ()
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (_emptyClassDefinitions);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (null)).IgnoreArguments().Return (_emptyRelationDefinitions);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (false);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);
      SetupResult.For (_mockMappingLoader.CreateClassDefinitionValidator()).Return (new ClassDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreatePropertyDefinitionValidator()).Return (new PropertyDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateRelationDefinitionValidator()).Return (new RelationDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateSortExpressionValidator()).Return (new SortExpressionValidator());

      _mockRepository.ReplayAll();

      var configuration = new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      _mockRepository.VerifyAll();

      MappingConfiguration.SetCurrent (configuration);
    }

    private void StubMockMappingLoader (ClassDefinition[] classDefinitions, RelationDefinition[] relationDefinitions)
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (classDefinitions);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (null)).IgnoreArguments().Return (relationDefinitions);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (true);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);
      SetupResult.For (_mockMappingLoader.CreateClassDefinitionValidator()).Return (new ClassDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreatePropertyDefinitionValidator()).Return (new PropertyDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateRelationDefinitionValidator()).Return (new RelationDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateSortExpressionValidator()).Return (new SortExpressionValidator());
    }

    private void StubMockMappingLoaderWithValidation (ClassDefinition[] classDefinitions, RelationDefinition[] relationDefinitions)
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (classDefinitions);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (null)).IgnoreArguments().Return (relationDefinitions);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (true);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);
      SetupResult.For (_mockMappingLoader.CreateClassDefinitionValidator()).Return (CreateClassDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreatePropertyDefinitionValidator()).Return (CreatePropertyDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateRelationDefinitionValidator()).Return (CreateRelationDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateSortExpressionValidator()).Return (CreateSortExpressionValidator());
    }

    private ClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator (new DomainObjectTypeIsNotGenericValidationRule());
    }

    private PropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator (new StorageClassIsSupportedValidationRule());
    }

    private RelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator (new RelationEndPointPropertyTypeIsSupportedValidationRule());
    }

    private SortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator (new SortExpressionIsValidValidationRule());
    }

    private IPersistenceModelLoader CreatePersistenceModelLoaderStub ()
    {
      var persistenceModelLoaderStub = _mockRepository.Stub<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Stub (stub => stub.ApplyPersistenceModelToHierarchy (Arg<ClassDefinition>.Is.Anything))
          .WhenCalled (
              mi => ((ClassDefinition) mi.Arguments[0]).SetStorageEntity (TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition)));
      persistenceModelLoaderStub
          .Stub (stub => stub.CreatePersistenceMappingValidator (Arg<ClassDefinition>.Is.Anything))
          .Return (new PersistenceMappingValidator());
      return persistenceModelLoaderStub;
    }
  }
}