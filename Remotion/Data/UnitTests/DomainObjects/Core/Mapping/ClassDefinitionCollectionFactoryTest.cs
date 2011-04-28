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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionFactoryTest
  {
    private Dictionary<Type, ClassDefinition> _classDefinitions;
    private ClassDefinitionCollectionFactory _classDefinitionCollectionFactory;
    private IMappingObjectFactory _mappingObjectFactoryMock;
    private ClassDefinition _fakeClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _classDefinitions = new Dictionary<Type, ClassDefinition>();
      _mappingObjectFactoryMock = MockRepository.GenerateStrictMock<IMappingObjectFactory>();
      _classDefinitionCollectionFactory = new ClassDefinitionCollectionFactory (_mappingObjectFactoryMock);
      _fakeClassDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
    }

    [Test]
    public void CreateClassDefinitionCollection ()
    {
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (Order), null))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay();

      var classDefinitions = _classDefinitionCollectionFactory.CreateClassDefinitionCollection (new[] { typeof (Order) });

      _mappingObjectFactoryMock.VerifyAllExpectations();

      Assert.That (classDefinitions, Is.EqualTo (new[] { _fakeClassDefinition }));
    }

    [Test]
    public void CreateClassDefinitionCollection_NoTypes ()
    {
      var classDefinitions = _classDefinitionCollectionFactory.CreateClassDefinitionCollection (new Type[0]);

      Assert.That (classDefinitions, Is.Empty);
    }

    [Test]
    public void CreateClassDefinitionCollection_DerivedClassAreSet ()
    {
      var fakeClassDefinitionCompany = ClassDefinitionFactory.CreateClassDefinition (typeof (Company));
      var fakeClassDefinitionPartner = ClassDefinitionFactory.CreateClassDefinition (typeof (Partner), fakeClassDefinitionCompany);
      var fakeClassDefinitionCustomer = ClassDefinitionFactory.CreateClassDefinition (typeof (Customer), fakeClassDefinitionCompany);

      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (Order), null))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (Company), null))
          .Return (fakeClassDefinitionCompany);
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (Partner), fakeClassDefinitionCompany))
          .Return (fakeClassDefinitionPartner);
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (Customer), fakeClassDefinitionCompany))
          .Return (fakeClassDefinitionCustomer);
      _mappingObjectFactoryMock.Replay();

      var classDefinitions =
          _classDefinitionCollectionFactory.CreateClassDefinitionCollection (
              new[] { typeof (Order), typeof (Company), typeof (Partner), typeof (Customer) });

      _mappingObjectFactoryMock.VerifyAllExpectations();
      
      Assert.That (classDefinitions.Length, Is.EqualTo (4));
      
      var orderClassDefinition = classDefinitions.Single (cd => cd.ClassType == typeof (Order));
      Assert.That (orderClassDefinition.DerivedClasses.Count, Is.EqualTo (0));

      var companyClassDefinition = classDefinitions.Single (cd => cd.ClassType == typeof (Company));
      Assert.That (companyClassDefinition.DerivedClasses, Is.EquivalentTo (new[] { fakeClassDefinitionPartner, fakeClassDefinitionCustomer }));
    }
    
    [Test]
    public void GetClassDefinition_ForDerivedClass_WithStorageGroupAttribute_IgnoresBaseClass ()
    {
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (ClassWithDifferentProperties), null))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay();

      var result = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (ClassWithDifferentProperties));

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForNonDerivedClass_WithoutStorageGroupAttribute ()
    {
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (ClassWithoutStorageGroupWithDifferentProperties), null))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay();

      var result = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (ClassWithoutStorageGroupWithDifferentProperties));

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForClassDerivedFromSimpleDomainObject_WithoutStorageGroupAttribute ()
    {
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (ClassDerivedFromSimpleDomainObject), null))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay ();

      var result = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (ClassDerivedFromSimpleDomainObject));

      _mappingObjectFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      var fakeBaseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithDifferentProperties));
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (ClassWithDifferentProperties), null))
          .Return (fakeBaseClassDefinition);
      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (DerivedClassWithDifferentProperties), fakeBaseClassDefinition))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay();

      var result = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (DerivedClassWithDifferentProperties));

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithBaseClassAlreadyInClassDefinitionCollection ()
    {
      var expectedBaseClass = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithDifferentProperties));
      _classDefinitions.Add (expectedBaseClass.ClassType, expectedBaseClass);

      _mappingObjectFactoryMock
          .Expect (mock => mock.CreateClassDefinition (typeof (DerivedClassWithDifferentProperties), expectedBaseClass))
          .Return (_fakeClassDefinition);
      _mappingObjectFactoryMock.Replay();

      var actual = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (DerivedClassWithDifferentProperties));

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _fakeClassDefinition);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      var existing = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      _classDefinitions.Add (existing.ClassType, existing);

      _mappingObjectFactoryMock.Replay();

      var actual = _classDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, typeof (Order));

      _mappingObjectFactoryMock.VerifyAllExpectations();
      Assert.IsNotNull (actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions[typeof (Order)]);
      Assert.AreSame (existing, actual);
    }
  }
}