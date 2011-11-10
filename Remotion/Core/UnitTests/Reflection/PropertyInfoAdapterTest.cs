// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class PropertyInfoAdapterTest
  {
    private PropertyInfo _property;
    private PropertyInfo _explicitInterfaceImplementationProperty;
    private PropertyInfo _implicitInterfaceImplementationProperty;

    private PropertyInfoAdapter _adapter;

    private PropertyInfoAdapter _explicitInterfaceAdapter;
    private PropertyInfoAdapter _implicitInterfaceAdapter;

    [SetUp]
    public void SetUp ()
    {
      var propertyInfoAdapterDataStore = 
          (IDataStore<PropertyInfo, PropertyInfoAdapter>) PrivateInvoke.GetNonPublicStaticField (typeof (PropertyInfoAdapter), "s_dataStore");
      propertyInfoAdapterDataStore.Clear();
      var methodInfoAdapterDataStore =
          (IDataStore<MethodInfo, MethodInfoAdapter>) PrivateInvoke.GetNonPublicStaticField (typeof (MethodInfoAdapter), "s_dataStore");
      methodInfoAdapterDataStore.Clear();

      _property = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      _adapter = PropertyInfoAdapter.Create(_property);

      _explicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceAdapter = PropertyInfoAdapter.Create(_explicitInterfaceImplementationProperty);

      _implicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "ImplicitInterfaceScalar",
          BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(_implicitInterfaceImplementationProperty);
    }

    [Test]
    public void Create_ReturnsSameInstance ()
    {
      Assert.That (PropertyInfoAdapter.Create (_property), Is.SameAs (PropertyInfoAdapter.Create (_property)));
    }

    [Test]
    public void Create_ReturnsSameInstance_ForMethodsReflectedFromDifferentLevelsInHierarchy ()
    {
      var propertyViaBase = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      var propertyViaDerived = typeof (DerivedClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");

      Assert.That (propertyViaBase, Is.Not.SameAs (propertyViaDerived));
      Assert.That (PropertyInfoAdapter.Create (propertyViaBase), Is.SameAs (PropertyInfoAdapter.Create (propertyViaDerived)));
    }

    [Test]
    public void PropertyInfo ()
    {
      Assert.That (_adapter.PropertyInfo, Is.SameAs (_property));
      Assert.That (_implicitInterfaceAdapter.PropertyInfo, Is.SameAs (_implicitInterfaceImplementationProperty));
      Assert.That (_explicitInterfaceAdapter.PropertyInfo, Is.SameAs (_explicitInterfaceImplementationProperty));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That (_adapter.PropertyType, Is.EqualTo (_property.PropertyType));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_property.Name));
    }

    [Test]
    public void Name_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo (_implicitInterfaceImplementationProperty.Name));
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo ("ImplicitInterfaceScalar"));
    }

    [Test]
    public void Name_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.Name, Is.EqualTo (_explicitInterfaceImplementationProperty.Name));
      Assert.That (
          _explicitInterfaceAdapter.Name,
          Is.EqualTo ("Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (TypeAdapter.Create (_property.DeclaringType)));
    }

    [Test]
    public void CanBeSetFromOutside_Scalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Scalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyScalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyScalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyNonPublicSetterScalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyNonPublicSetterScalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitInterfaceScalar_FromImplementation ()
    {
      PropertyInfoAdapter adapter = _implicitInterfaceAdapter;

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (
          _adapter.GetCustomAttribute<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttribute<SampleAttribute> (_property, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (
          _adapter.GetCustomAttributes<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttributes<SampleAttribute> (_property, true)));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SampleAttribute> (_property, true)));
    }

    [Test]
    public void GetValue_WithIndexerProperty_OneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10 });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_OneParameter_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();
      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[0]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_TwoParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });

      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1) });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayNull ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[1]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_ThreeParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValue_ExplicitInterface_Integration ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType>();
      SimpleReferenceType value = new SimpleReferenceType();
      _explicitInterfaceAdapter.SetValue (instance, value, null);
      Assert.That (instance.ExplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithOneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10 });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[0]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithTwoParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1) });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[1]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithThreeParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = PropertyInfoAdapter.Create(interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_adapter.DeclaringType));

      PropertyInfo propertyInfo = typeof (ClassWithOverridingMember).GetProperty ("BaseProperty");
      PropertyInfoAdapter overrideAdapter = PropertyInfoAdapter.Create(propertyInfo);
      Assert.AreNotEqual (overrideAdapter.DeclaringType, overrideAdapter.GetOriginalDeclaringType());
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (overrideAdapter.DeclaringType.BaseType));
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (typeof (ClassWithBaseMember)));
    }

    [Test]
    public void GetGetMethod_PublicProperty ()
    {
      var getMethod = _adapter.GetGetMethod (false);
      var expectedMethod = MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("get_NotVisibleAttributeScalar"));

      Assert.That (getMethod, Is.Not.Null);
      Assert.That (getMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetGetMethod_PrivateProperty_NonPublicFalse ()
    {
      var getMethod = _explicitInterfaceAdapter.GetGetMethod (false);

      Assert.That (getMethod, Is.Null);
    }

    [Test]
    public void GetGetMethod_PrivateProperty_NonPublicTrue ()
    {
      var getMethod = _explicitInterfaceAdapter.GetGetMethod (true);

      var expectedMethod = MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      Assert.That (getMethod, Is.Not.Null);
      Assert.That (getMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetGetMethod_NonExistingMethod ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceWriteOnlyScalar"));

      var getMethod = adapter.GetGetMethod (false);

      Assert.That (getMethod, Is.Null);
    }

    [Test]
    public void GetSetMethod_PublicProperty ()
    {
      var setMethod = _adapter.GetSetMethod (false);

      var expectedMethod = MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("set_NotVisibleAttributeScalar"));
      Assert.That (setMethod, Is.Not.Null);
      Assert.That (setMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetSetMethod_PrivateProperty_NonPublicFalse ()
    {
      var setMethod = _explicitInterfaceAdapter.GetSetMethod (false);

      Assert.That (setMethod, Is.Null);
    }

    [Test]
    public void GetSetMethod_PrivateProperty_NonPublicTrue ()
    {
      var setMethod = _explicitInterfaceAdapter.GetSetMethod (true);

      var expectedMethod = MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.set_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      Assert.That (setMethod, Is.Not.Null);
      Assert.That (setMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetSetMethod_NonExistingMethod ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceReadOnlyScalar"));

      var setMethod = adapter.GetSetMethod (false);

      Assert.That (setMethod, Is.Null);
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ImplicitImplementation ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      Assert.That (
          ((PropertyInfoAdapter) implementation).PropertyInfo,
          Is.EqualTo (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_OnBaseClass ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_GetterOnly ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceReadOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_SetterOnly ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceWriteOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceWriteOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ImplementedOnBaseClass ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceReadOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (
          ((PropertyInfoAdapter) implementation).PropertyInfo,
          Is.EqualTo (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceReadOnlyScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_OverriddenOnDerivedClass ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (
          ((PropertyInfoAdapter) implementation).PropertyInfo,
          Is.EqualTo (typeof (DerivedClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_NotImplemented ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      var implementation = adapter.FindInterfaceImplementation (typeof (object));

      Assert.That (implementation, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The implementationType parameter must not be an interface.\r\nParameter name: implementationType")]
    public void FindInterfaceImplementation_InterfaceProperty_ImplementationIsInterface ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      adapter.FindInterfaceImplementation (typeof (IInterfaceWithReferenceType<object>));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This property is not an interface property.")]
    public void FindInterfaceImplementation_NonInterfaceProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));
    }

    [Test]
    public void FindInterfaceDeclaration_ImplicitInterfaceImplementation ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (result.Name, Is.EqualTo ("ImplicitInterfaceScalar"));
      Assert.That (result.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (IInterfaceWithReferenceType<object>))));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitInterfaceImplementation ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (result.Name, Is.EqualTo ("ExplicitInterfaceScalar"));
      Assert.That (result.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (IInterfaceWithReferenceType<object>))));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitInterfaceImplementation_ReadOnlyProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (result.Name, Is.EqualTo ("ExplicitInterfaceReadOnlyScalar"));
      Assert.That (result.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (IInterfaceWithReferenceType<object>))));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitInterfaceImplementation_WriteOnlyProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceWriteOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (result.Name, Is.EqualTo ("ExplicitInterfaceWriteOnlyScalar"));
      Assert.That (result.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (IInterfaceWithReferenceType<object>))));
    }

    [Test]
    public void FindInterfaceDeclaration_NonInterfaceImplementationProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty ("Scalar"));

      Assert.That (adapter.FindInterfaceDeclaration(), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This property is itself an interface member, so it cannot have an interface declaration.")]
    public void FindInterfaceDeclaration_InterfaceProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      adapter.FindInterfaceDeclaration();
    }

    [Test]
    public void GetIndexParameters_IndexedProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (ClassWithReferenceType<object>).GetProperty ("Item", new[] { typeof (int) }));

      Assert.That (adapter.GetIndexParameters().Length, Is.EqualTo (1));
    }

    [Test]
    public void GetIndexParameters_NoIndexedProperty ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      Assert.That (adapter.GetIndexParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetAccessors_GetterAndSetter ()
    {
      var propertyInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      var result = adapter.GetAccessors (false);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (
          ((MethodInfoAdapter) result[0]).MethodInfo, Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar")));
      Assert.That (
          ((MethodInfoAdapter) result[1]).MethodInfo, Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("set_ImplicitInterfaceScalar")));
    }

    [Test]
    public void GetAccessors_Getter ()
    {
      var propertyInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceReadOnlyScalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      var result = adapter.GetAccessors (false);

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (
          ((MethodInfoAdapter) result[0]).MethodInfo,
          Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceReadOnlyScalar")));
    }

    [Test]
    public void GetAccessors_Setter ()
    {
      var propertyInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceWriteOnlyScalar");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      var result = adapter.GetAccessors (false);

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (
          ((MethodInfoAdapter) result[0]).MethodInfo,
          Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("set_ImplicitInterfaceWriteOnlyScalar")));
    }

    [Test]
    public void GetAccessors_NonPublicSetter_NonPublicFalse ()
    {
      var propertyInfo = typeof (ClassWithReferenceType<object>).GetProperty ("PropertyWithPrivateSetter");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      var result = adapter.GetAccessors (false);

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (
          ((MethodInfoAdapter) result[0]).MethodInfo,
          Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("get_PropertyWithPrivateSetter")));
    }

    [Test]
    public void GetAccessors_NonPublicSetter_NonPublicTrue ()
    {
      var propertyInfo = typeof (ClassWithReferenceType<object>).GetProperty ("PropertyWithPrivateSetter");
      var adapter = PropertyInfoAdapter.Create(propertyInfo);

      var result = adapter.GetAccessors (true);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (
          ((MethodInfoAdapter) result[0]).MethodInfo,
          Is.SameAs (typeof (ClassWithReferenceType<object>).GetMethod ("get_PropertyWithPrivateSetter")));
      Assert.That (
          ((MethodInfoAdapter) result[1]).MethodInfo,
          Is.SameAs (
              typeof (ClassWithReferenceType<object>).GetMethod ("set_PropertyWithPrivateSetter", BindingFlags.Instance | BindingFlags.NonPublic)));
    }

    [Test]
    public void To_String ()
    {
      var adapter = PropertyInfoAdapter.Create(typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      Assert.That (adapter.ToString(), Is.EqualTo ("System.Object ImplicitInterfaceScalar"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_adapter.Equals (null), Is.False);
      Assert.That (_adapter.Equals ("test"), Is.False);
      Assert.That (_implicitInterfaceAdapter.Equals (PropertyInfoAdapter.Create(_explicitInterfaceImplementationProperty)), Is.False);

      Assert.That (_implicitInterfaceAdapter.Equals (PropertyInfoAdapter.Create(_implicitInterfaceImplementationProperty)), Is.True);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter.GetHashCode(),
          Is.EqualTo (PropertyInfoAdapter.Create(_implicitInterfaceImplementationProperty).GetHashCode()));
      Assert.That (
          PropertyInfoAdapter.Create(typeof (int[]).GetProperty ("Length")).GetHashCode(),
          Is.EqualTo (PropertyInfoAdapter.Create(typeof (int[]).GetProperty ("Length")).GetHashCode()));
    }

    [Test]
    public void IsSupportedByTypeConversionProvider ()
    {
      var typeConversionProvider = TypeConversionProvider.Create ();

      Assert.That (typeConversionProvider.CanConvert (typeof (PropertyInfoAdapter), typeof (PropertyInfo)), Is.True);
    }

    private void AssertCanSet (PropertyInfoAdapter adapter, object instance, SimpleReferenceType value)
    {
      adapter.SetValue (instance, value, null);
      Assert.That (adapter.GetValue (instance, null), Is.SameAs (value));
    }

    private void AssertCanNotSet (PropertyInfoAdapter adapter, object instance, SimpleReferenceType value)
    {
      try
      {
        adapter.SetValue (instance, value, null);
      }
      catch (ArgumentException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Property set method not found."));
      }
    }
  }
}