// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectParticipantTest
  {
    private ITypeDefinitionProvider _typeDefinitionProviderMock;

    private DomainObjectParticipant _participant;
    private IInterceptedPropertyFinder _interceptedPropertyFinderMock;
    private IRelatedMethodFinder _relatedMethodFinderMock;

    private ProxyType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _typeDefinitionProviderMock = MockRepository.GenerateStrictMock<ITypeDefinitionProvider>();
      _interceptedPropertyFinderMock = MockRepository.GenerateStrictMock<IInterceptedPropertyFinder>();
      _relatedMethodFinderMock = MockRepository.GenerateStrictMock<IRelatedMethodFinder>();

      _participant = new DomainObjectParticipant (_typeDefinitionProviderMock, _interceptedPropertyFinderMock, _relatedMethodFinderMock);

      _proxyType = ProxyTypeObjectMother.Create (typeof (ConcreteBaseType));
    }

    [Test]
    public void PartialCacheKeyProvider ()
    {
      var cacheKeyProvider = _participant.PartialCacheKeyProvider;
      // Retrieving the property does not cause any calls to the mock objects.

      var requestedType = ReflectionObjectMother.GetSomeType();
      var fakePublicDomainType = ReflectionObjectMother.GetSomeDifferentType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (requestedType)).Return (fakePublicDomainType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakePublicDomainType)).Return (fakeClassDefinition);

      var result = cacheKeyProvider.GetCacheKey (requestedType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeClassDefinition));
    }

    [Test]
    public void ModifyType_AddMarkerInterface_And_OverrideHooks ()
    {
      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var fakeProperties = Enumerable.Empty<Tuple<PropertyInfo, string>>();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (typeof (ConcreteBaseType))).Return (fakeDomainObjectType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakeDomainObjectType)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Expect (mock => mock.GetProperties (fakeDomainObjectType)).Return (fakeProperties);

      _participant.ModifyType (_proxyType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      _interceptedPropertyFinderMock.VerifyAllExpectations();
      Assert.That (_proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (IInterceptedDomainObject) }));
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));

      var performConstructorCheck = _proxyType.AddedMethods.Single (m => m.Name == "PerformConstructorCheck");
      var getPublicDomainObjectTypeImplementation = _proxyType.AddedMethods.Single (m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That (performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property ("Type").SameAs (typeof (void)));
      Assert.That (getPublicDomainObjectTypeImplementation.Body, Is.TypeOf<ConstantExpression>().And.Property ("Value").SameAs (fakeDomainObjectType));
    }

    [Test]
    public void ModifyType_UsesCorrectMethodInOverrideHierarchy_And_SkipPropertyProcessingIfNotOverridable ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((MyDomainObject o) => o.SomeProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      var getterBaseDefinition = getter.GetBaseDefinition();
      var setterBaseDefinition = setter.GetBaseDefinition();
      var getterOverride = typeof (ConcreteBaseType).GetMethod ("get_SomeProperty");
      var setterOverride = typeof (ConcreteBaseType).GetMethod ("set_SomeProperty");
      Assert.That (getter, Is.Not.EqualTo (getterBaseDefinition).And.Not.EqualTo (getterOverride));
      Assert.That (setter, Is.Not.EqualTo (setterBaseDefinition).And.Not.EqualTo (setterOverride));

      var fakeProperties = new[] { Tuple.Create (property, "abc") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (getterBaseDefinition, _proxyType)).Return (getterOverride);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (setterBaseDefinition, _proxyType)).Return (setterOverride);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsOverridable (getterOverride)).Return (false);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsOverridable (setterOverride)).Return (false);

      _participant.ModifyType (_proxyType);

      _interceptedPropertyFinderMock.VerifyAllExpectations();
      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));
    }

    [Test]
    public void ModifyType_ImplementProperties ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((ConcreteBaseType o) => o.SomeProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();

      var fakeProperties = new[] { Tuple.Create (property, "propertyIdentifier") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Stub (stub => stub.GetMostDerivedOverride (Arg<MethodInfo>.Is.Anything, Arg<Type>.Is.Anything)).Return (getter).Repeat.Once();
      _relatedMethodFinderMock.Stub (stub => stub.GetMostDerivedOverride (Arg<MethodInfo>.Is.Anything, Arg<Type>.Is.Anything)).Return (setter);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsOverridable (Arg<MethodInfo>.Is.Anything)).Return (true);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsAutomaticPropertyAccessor (Arg<MethodInfo>.Is.Anything)).Return (true);

      _participant.ModifyType (_proxyType);

      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (4));
      var addedGetter = _proxyType.AddedMethods.Single (m => m.Name == "get_SomeProperty");
      var addedSetter = _proxyType.AddedMethods.Single (m => m.Name == "set_SomeProperty");

      var propertyAccessor =
          Expression.Call (
              Expression.Property (new ThisExpression (_proxyType), "Properties"),
              "get_Item",
              null,
              Expression.Constant ("propertyIdentifier"));
      var expectedGetterBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant ("propertyIdentifier")),
              Expression.TryFinally (
                  Expression.Call (
                      propertyAccessor,
                      "GetValue",
                      new[] { typeof (string) }),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));
      var expectedSetterBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant ("propertyIdentifier")),
              Expression.TryFinally (
                  Expression.Call (
                      propertyAccessor,
                      "SetValue",
                      new[] { typeof (string) },
                      Expression.Parameter (typeof (string), "value")),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedGetterBody, addedGetter.Body);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSetterBody, addedSetter.Body);
    }

    [Test]
    public void ModifyType_WrapProperties ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((ConcreteBaseType o) => o.SomeProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();

      var fakeProperties = new[] { Tuple.Create (property, "abc") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Stub (stub => stub.GetMostDerivedOverride (Arg<MethodInfo>.Is.Anything, Arg<Type>.Is.Anything)).Return (getter).Repeat.Once();
      _relatedMethodFinderMock.Stub (stub => stub.GetMostDerivedOverride (Arg<MethodInfo>.Is.Anything, Arg<Type>.Is.Anything)).Return (setter);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsOverridable (Arg<MethodInfo>.Is.Anything)).Return (true);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsAutomaticPropertyAccessor (Arg<MethodInfo>.Is.Anything)).Return (false);

      _participant.ModifyType (_proxyType);

      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (4));
      var addedGetter = _proxyType.AddedMethods.Single (m => m.Name == "get_SomeProperty");
      var addedSetter = _proxyType.AddedMethods.Single (m => m.Name == "set_SomeProperty");

      var expectedGetterBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant ("abc")),
              Expression.TryFinally (
                  Expression.Call (new ThisExpression (_proxyType), NonVirtualCallMethodInfoAdapter.Adapt (getter)),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));
      var expectedSetterBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant ("abc")),
              Expression.TryFinally (
                  Expression.Call (
                      new ThisExpression (_proxyType),
                      NonVirtualCallMethodInfoAdapter.Adapt (setter),
                      Expression.Parameter (typeof (string), "value")),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedGetterBody, addedGetter.Body);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSetterBody, addedSetter.Body);
    }

    [Test]
    public void ModifyType_NonPublicAccessorsAreConsidered ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((MyDomainObject o) => o.InternalProperty);
      var getter = property.GetGetMethod (true);
      var setter = property.GetSetMethod (true);
      Assert.That (getter.IsPublic, Is.False);
      Assert.That (setter.IsPublic, Is.False);

      var fakeProperties = new[] { Tuple.Create (property, "abc") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (getter, _proxyType)).Return (getter);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (setter, _proxyType)).Return (setter);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsOverridable (Arg<MethodInfo>.Is.Anything)).Return (false);

      _participant.ModifyType (_proxyType);

      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));
    }

    [Test]
    public void ModifyType_ReadOnlyProperty_And_WriteOnlyProperty_AreSupported ()
    {
      var readOnlyProperty = NormalizingMemberInfoFromExpressionUtility.GetProperty ((MyDomainObject o) => o.ReadOnlyProperty);
      var writeOnlyProperty = typeof (MyDomainObject).GetProperty ("WriteOnlyProperty");
      var getter = readOnlyProperty.GetGetMethod();
      var setter = writeOnlyProperty.GetSetMethod();
      Assert.That (readOnlyProperty.GetSetMethod (true), Is.Null);
      Assert.That (writeOnlyProperty.GetGetMethod (true), Is.Null);

      var fakeProperties = new[] { Tuple.Create (readOnlyProperty, "abc"), Tuple.Create (writeOnlyProperty, "def") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (getter, _proxyType)).Return (getter);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (setter, _proxyType)).Return (setter);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsOverridable (Arg<MethodInfo>.Is.Anything)).Return (false);

      _participant.ModifyType (_proxyType);

      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));
    }

    [Test]
    public void ModifyType_UsesLastArgumentForValue ()
    {
      var property = typeof (MyDomainObject).GetProperty ("Item");
      var indexedSetter = property.GetSetMethod();
      Assert.That (indexedSetter.GetParameters().Select (p => p.ParameterType), Is.EqualTo (new[] { typeof (int), typeof (int), typeof (double) }));

      var fakeProperties = new[] { Tuple.Create (property, "abc") };
      StubGetProperties (fakeProperties);
      _relatedMethodFinderMock.Stub (stub => stub.GetMostDerivedOverride (Arg<MethodInfo>.Is.Anything, Arg<Type>.Is.Anything)).Return (indexedSetter);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsOverridable (Arg<MethodInfo>.Is.Anything)).Return (true);
      _interceptedPropertyFinderMock.Stub (stub => stub.IsAutomaticPropertyAccessor (Arg<MethodInfo>.Is.Anything)).Return (true);

      _participant.ModifyType (_proxyType);

      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (3));
      var addedSetter = _proxyType.AddedMethods.Single (m => m.Name == "set_Item");

      var propertyAccessor =
          Expression.Call (
              Expression.Property (new ThisExpression (_proxyType), "Properties"),
              "get_Item",
              null,
              Expression.Constant ("abc"));
      var expectedSetterBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant ("abc")),
              Expression.TryFinally (
                  Expression.Call (
                      propertyAccessor,
                      "SetValue",
                      new[] { typeof (double) },
                      Expression.Parameter (typeof (double), "value")),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedSetterBody, addedSetter.Body);
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type 'System.Int32' as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.")]
    public void ModifyType_ThrowsForAbstractClassDefinition ()
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (int), isAbstract: true);
      _typeDefinitionProviderMock.Stub (stub => stub.GetPublicDomainObjectType (Arg<Type>.Is.Anything));
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (Arg<Type>.Is.Anything)).Return (fakeClassDefinition);

      _participant.ModifyType (_proxyType);
    }

    private void StubGetProperties (IEnumerable<Tuple<PropertyInfo, string>> fakeProperties)
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Stub (stub => stub.GetPublicDomainObjectType (Arg<Type>.Is.Anything));
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (Arg<Type>.Is.Anything)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Stub (stub => stub.GetProperties (Arg<Type>.Is.Anything)).Return (fakeProperties);
    }

    // TODO 5370: This is only here for the accesser.GetBaseDefinition() in the call to RelatedMethoFinder.GetMostDerivedOverride.
    [IgnoreForMappingConfiguration]
    private class MyDomainObjectBase : DomainObject
    {
      public virtual string SomeProperty { get; set; }
    }

    [IgnoreForMappingConfiguration]
    private class MyDomainObject : MyDomainObjectBase
    {
      public override string SomeProperty { get; set; }
      protected internal string InternalProperty { get; [UsedImplicitly] set; }
      public string ReadOnlyProperty { get { return ""; } }
      [UsedImplicitly] public string WriteOnlyProperty { set { Dev.Null = value; } }
      public virtual double this[int i1, int i2] { set { Dev.Null = i1; Dev.Null = i2; Dev.Null = value; } }
    }

    // TODO 5370: 'ConcreteBaseType' not needed after TypePipe integration with re-mix.
    [IgnoreForMappingConfiguration]
    private class ConcreteBaseType : MyDomainObject
    {
      public override string SomeProperty { get; set; }
    }
  }
}