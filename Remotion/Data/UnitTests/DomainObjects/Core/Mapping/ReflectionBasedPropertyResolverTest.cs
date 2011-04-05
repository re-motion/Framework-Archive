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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.IntegrationTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedPropertyResolverTest : MappingReflectionTestBase
  {
    private ClassDefinition _orderClass;

    private ClassDefinition _targetClassForPersistentMixinClass;
    private ClassDefinition _derivedTargetClassForPersistentMixinClass;

    private ClassDefinition _classWithInterface;
    private ClassDefinition _classDerivedFromClassWithInterface;
    private ClassDefinition _classWithInterfaceWithMissingAccessors;
    private ClassDefinition _classWithMixinAddingInterface;
    private ClassDefinition _derivedClassWithMixinWithDuplicateInterface;

    public override void SetUp ()
    {
      base.SetUp();

      _orderClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];

      _targetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.ClassDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.ClassDefinitions[typeof (DerivedTargetClassForPersistentMixin)];
      
      _classWithInterface = CreateDefinitionForClassWithInterface();
      _classDerivedFromClassWithInterface = CreateDefinitionForClassDerivedFromClassWithInterface(_classWithInterface);
      _classDerivedFromClassWithInterface.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithInterfaceWithMissingAccessors = CreateDefinitionForClassWithInterfaceWithMissingAccessors();
      _classWithMixinAddingInterface = CreateDefinitionForClassWithMixinAddingInterface();
      _derivedClassWithMixinWithDuplicateInterface = CreateDefinitionForDerivedClassWithMixinWithDuplicateInterface();
    }

    [Test]
    public void ResolveDefinition ()
    {
      var property = new PropertyInfoAdapter(typeof (Order).GetProperty ("OrderNumber"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (property, _orderClass, _orderClass.GetPropertyDefinition);

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_StorageClassNoneProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (Order).GetProperty ("RedirectedOrderNumber"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (property, _orderClass, _orderClass.GetPropertyDefinition);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithProperties).GetProperty ("ImplicitProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (typeof (ClassWithInterface).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromInterfaceProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithProperties).GetProperty ("ExplicitManagedProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterface).FullName + "." + typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromInterfaceProperty_FromDerivedClass()
    {
      var property = new PropertyInfoAdapter (typeof(IInterfaceWithProperties).GetProperty("ExplicitManagedProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition>(
          property, _classDerivedFromClassWithInterface, _classDerivedFromClassWithInterface.GetPropertyDefinition);

      var expected = _classDerivedFromClassWithInterface.GetMandatoryPropertyDefinition(
          typeof(ClassWithInterface).FullName + "." + typeof(IInterfaceWithProperties).FullName + ".ExplicitManagedProperty");
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromImplementationProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (ClassWithInterface).GetProperty ("ImplicitProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (typeof (ClassWithInterface).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_GetAccessorOnly ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithPropertiesWithMissingAccessors).GetProperty ("PropertyWithGetAccessor"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterfaceWithMissingAccessors, _classWithInterfaceWithMissingAccessors.GetPropertyDefinition);

      var expected = _classWithInterfaceWithMissingAccessors.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterfaceWithMissingAccessors).FullName + ".PropertyWithGetAccessor");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_SetAccessorOnly ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithPropertiesWithMissingAccessors).GetProperty ("PropertyWithSetAccessor"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterfaceWithMissingAccessors, _classWithInterfaceWithMissingAccessors.GetPropertyDefinition);

      var expected = _classWithInterfaceWithMissingAccessors.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterfaceWithMissingAccessors).FullName + ".PropertyWithSetAccessor");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_NotImplemented ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ImplicitProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition); // does not implement this property

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromImplementationProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (ClassWithInterface).GetProperty (
          typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty",
          BindingFlags.Instance | BindingFlags.NonPublic));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterface).FullName + "." + typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromInterfaceProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ImplicitProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected =
          _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (typeof (MixinAddingInterfaceWithProperties).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromInterfaceProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ExplicitManagedProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected = _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (
          typeof (MixinAddingInterfaceWithProperties).FullName + "." + typeof (IInterfaceWithPropertiesAddedByMixin).FullName
          + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromImplementationProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (MixinAddingInterfaceWithProperties).GetProperty ("ImplicitProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected =
          _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (typeof (MixinAddingInterfaceWithProperties).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromImplementationProperty ()
    {
      var property =  new PropertyInfoAdapter(typeof (MixinAddingInterfaceWithProperties).GetProperty (
          typeof (IInterfaceWithPropertiesAddedByMixin).FullName + ".ExplicitManagedProperty",
          BindingFlags.Instance | BindingFlags.NonPublic));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected = _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (
          typeof (MixinAddingInterfaceWithProperties).FullName + "." + typeof (IInterfaceWithPropertiesAddedByMixin).FullName
          + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinPropertyOnBaseClass ()
    {
      var property =  new PropertyInfoAdapter(typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _derivedTargetClassForPersistentMixinClass, _derivedTargetClassForPersistentMixinClass.GetPropertyDefinition);

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinWithDuplicateInterface ()
    {
      var property = new PropertyInfoAdapter (typeof (IMixinAddingProperty).GetProperty ("Property"));

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _derivedClassWithMixinWithDuplicateInterface, _derivedClassWithMixinWithDuplicateInterface.GetPropertyDefinition);

      var expected = _derivedClassWithMixinWithDuplicateInterface.GetPropertyDefinition (typeof (MixinAddingPropertyBase).FullName + ".Property");
      Assert.That (result, Is.SameAs (expected));
    }

    private ClassDefinition CreateDefinitionForClassWithInterface ()
    {
      var classWithInterface = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithInterface));
      var properties = new List<PropertyDefinition>();
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithInterface,
              typeof (ClassWithInterface),
              "ImplicitProperty",
              "ImplicitProperty",
              typeof (string),
              false,
              100));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithInterface,
              typeof (ClassWithInterface),
              typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty",
              "ExplicitManagedProperty",
              typeof (string),
              false,
              100));
      classWithInterface.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
      return classWithInterface;
    }

    private ClassDefinition CreateDefinitionForClassDerivedFromClassWithInterface(ClassDefinition baseClassDefinition)
    {
      Type type = typeof (ClassDerivedFromClassWithInterface);
      return ClassDefinitionFactory.CreateClassDefinition(type.Name, type.Name, UnitTestDomainStorageProviderDefinition, type, false, baseClassDefinition);
    }

    private ClassDefinition CreateDefinitionForClassWithInterfaceWithMissingAccessors ()
    {
      var classWithInterfaceWithMissingAccessors =
          ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithInterfaceWithMissingAccessors));
      var properties = new List<PropertyDefinition>();
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithInterfaceWithMissingAccessors,
              typeof (ClassWithInterfaceWithMissingAccessors),
              "PropertyWithGetAccessor",
              "PropertyWithGetAccessor",
              typeof (string),
              false,
              100));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithInterfaceWithMissingAccessors,
              typeof (ClassWithInterfaceWithMissingAccessors),
              "PropertyWithSetAccessor",
              "PropertyWithSetAccessor",
              typeof (string),
              false,
              100));
      classWithInterfaceWithMissingAccessors.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
      return classWithInterfaceWithMissingAccessors;
    }

    private ClassDefinition CreateDefinitionForClassWithMixinAddingInterface ()
    {
      var classWithMixinAddingInterface =
          ClassDefinitionFactory.CreateClassDefinition (typeof (TargetClassForMixinAddingInterfaceWithProperties), typeof (MixinAddingInterfaceWithProperties));
      var properties = new List<PropertyDefinition>();
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithMixinAddingInterface,
              typeof (MixinAddingInterfaceWithProperties),
              "ImplicitProperty",
              "ImplicitProperty",
              typeof (string),
              false,
              100));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classWithMixinAddingInterface,
              typeof (MixinAddingInterfaceWithProperties),
              typeof (IInterfaceWithPropertiesAddedByMixin).FullName + ".ExplicitManagedProperty",
              "ExplicitManagedProperty",
              typeof (string),
              false,
              100));
      classWithMixinAddingInterface.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
      return classWithMixinAddingInterface;
    }

    private ClassDefinition CreateDefinitionForDerivedClassWithMixinWithDuplicateInterface ()
    {
      var classWithMixinAddingInterface =
          ClassDefinitionFactory.CreateClassDefinition (typeof (DerivedClassWithMixinWithDuplicateInterface), typeof (MixinAddingProperty), typeof(MixinAddingPropertyBase));
      classWithMixinAddingInterface.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{
          PropertyDefinitionFactory.Create (
              classWithMixinAddingInterface,
              typeof (MixinAddingPropertyBase),
              "Property",
              "Property",
              typeof (string),
              false,
              100)}, true));
      return classWithMixinAddingInterface;
    }
  }
}