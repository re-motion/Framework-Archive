/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyInfoAdapterTestDomain;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class PropertyInfoAdapterTest
  {
    private PropertyInfo _property;
    private PropertyInfo _explicitInterfaceImplementationProperty;
    private PropertyInfo _explicitInterfaceDeclarationProperty;
    private PropertyInfo _implicitInterfaceImplementationProperty;
    private PropertyInfo _implicitInterfaceDeclarationProperty;

    private PropertyInfoAdapter _adapter;

    private PropertyInfoAdapter _explicitInterfaceAdapter;
    private PropertyInfoAdapter _implicitInterfaceAdapter;

    [SetUp]
    public void SetUp ()
    {
      _property = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      _adapter = new PropertyInfoAdapter (_property, null);

      _explicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ExplicitInterfaceScalar");
      _explicitInterfaceAdapter = new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty);

      _implicitInterfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      _implicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "ImplicitInterfaceScalar",
          BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty);
    }

    [Test]
    public void PropertyInfo ()
    {
      Assert.That (_adapter.PropertyInfo, Is.SameAs (_property));
      Assert.That (_implicitInterfaceAdapter.PropertyInfo, Is.SameAs (_implicitInterfaceImplementationProperty));
      Assert.That (_explicitInterfaceAdapter.PropertyInfo, Is.SameAs (_explicitInterfaceImplementationProperty));
    }

    [Test]
    public void InterfacePropertyInfo ()
    {
      Assert.That (_adapter.InterfacePropertyInfo, Is.Null);
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_implicitInterfaceDeclarationProperty));
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_explicitInterfaceDeclarationProperty));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter must be a property declared on an interface.\r\nParameter name: interfacePropertyInfo")]
    public void InvalidInterfaceProperty ()
    {
      new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceImplementationProperty);
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
      Assert.That (_explicitInterfaceAdapter.Name, 
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (_property.DeclaringType));
    }

    [Test]
    public void CanBeSetFromOutside ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Scalar");
      PropertyInfoAdapter readWriteAdapter = new PropertyInfoAdapter (propertyInfo);
      Assert.That (readWriteAdapter.CanBeSetFromOutside, Is.True);
      PropertyInfo propertyInfo1 = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyScalar");
      PropertyInfoAdapter readOnlyAdapter = new PropertyInfoAdapter (propertyInfo1);
      Assert.That (readOnlyAdapter.CanBeSetFromOutside, Is.False);

      PropertyInfo propertyInfo2 = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyNonPublicSetterScalar");
      PropertyInfoAdapter readOnlyNonPublicSetterAdapter = new PropertyInfoAdapter (propertyInfo2);
      Assert.That (readOnlyNonPublicSetterAdapter.CanBeSetFromOutside, Is.False);

      PropertyInfoAdapter readWriteExplicitAdapter = _explicitInterfaceAdapter;
      Assert.That (readWriteExplicitAdapter.CanBeSetFromOutside, Is.True);

      PropertyInfo propertyInfo3 = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar", BindingFlags.NonPublic | BindingFlags.Instance);
      PropertyInfoAdapter readOnlyExplicitAdapter = new PropertyInfoAdapter (propertyInfo3);
      Assert.That (readOnlyExplicitAdapter.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (
          _adapter.GetCustomAttribute<ObjectBindingAttribute> (true),
          Is.EqualTo (
              AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (_property, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (
          _adapter.GetCustomAttributes<ObjectBindingAttribute> (true),
          Is.EqualTo (
              AttributeUtility.GetCustomAttributes<ObjectBindingAttribute> (_property, true)));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<ObjectBindingAttribute> (true),
          Is.EqualTo (
              AttributeUtility.IsDefined<ObjectBindingAttribute> (_property, true)));
    }

    [Test]
    public void GetValue_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock.ImplicitInterfaceScalar).Return (scalar);
      instanceMock.Replay();

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, null);
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValue_ExplicitInterface_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType ();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock.ExplicitInterfaceScalar).Return (scalar);
      instanceMock.Replay ();

      object actualScalar = _explicitInterfaceAdapter.GetValue (instanceMock, null);
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetValue_ExplicitInterface_Integration ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      instance.ExplicitInterfaceScalar = new SimpleReferenceType ();
      Assert.That (_explicitInterfaceAdapter.GetValue (instance, null), Is.EqualTo (instance.ExplicitInterfaceScalar));
    }
    
    [Test]
    public void SetValue_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType ();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock.ImplicitInterfaceScalar = scalar);
      instanceMock.Replay ();

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, null);
      instanceMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetValue_ExplicitInterface_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType ();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock.ExplicitInterfaceScalar = scalar);
      instanceMock.Replay ();

      _explicitInterfaceAdapter.SetValue (instanceMock, scalar, null);
      instanceMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetValue_ExplicitInterface_Integration ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      SimpleReferenceType value = new SimpleReferenceType ();
      _explicitInterfaceAdapter.SetValue (instance, value, null);
      Assert.That (instance.ExplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void Equals_ChecksPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter,
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty)));
      Assert.AreNotEqual (new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty), _implicitInterfaceAdapter);
    }

    [Test]
    public void Equals_ChecksValuePropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter,
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty)));
      Assert.AreNotEqual (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty), _implicitInterfaceAdapter);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter.GetHashCode(),
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty).GetHashCode()));
      Assert.AreNotEqual (new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty).GetHashCode (), _implicitInterfaceAdapter.GetHashCode ());
      Assert.AreNotEqual (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty).GetHashCode (), _implicitInterfaceAdapter.GetHashCode ());
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_adapter.DeclaringType));

      PropertyInfo propertyInfo = typeof (ClassWithOverridingProperty).GetProperty ("BaseProperty");
      PropertyInfoAdapter overrideAdapter = new PropertyInfoAdapter (propertyInfo);
      Assert.AreNotEqual (overrideAdapter.DeclaringType, overrideAdapter.GetOriginalDeclaringType ());
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (overrideAdapter.DeclaringType.BaseType));
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (typeof (ClassWithBaseProperty)));
    }

    [Test]
    public void InterfaceProperty_Null ()
    {
      Assert.That (_adapter.InterfacePropertyInfo, Is.Null);
    }

    [Test]
    public void DeclaringInterfaceType_NonNull_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.Not.Null);
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_explicitInterfaceDeclarationProperty));
    }

    [Test]
    public void DeclaringInterfaceType_NonNull_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.Not.Null);
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_implicitInterfaceDeclarationProperty));
    }
  }
}
