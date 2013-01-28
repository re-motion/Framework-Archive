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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class InterceptedDomainObjectParticipantTest
  {
    private ITypeDefinitionProvider _typeDefinitionProviderMock;

    private InterceptedDomainObjectParticipant _participant;
    private IInterceptedPropertyFinder _interceptedPropertyFinderMock;
    private IRelatedMethodFinder _relatedMethodFinderMock;

    private ProxyType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _typeDefinitionProviderMock = MockRepository.GenerateStrictMock<ITypeDefinitionProvider>();
      _interceptedPropertyFinderMock = MockRepository.GenerateStrictMock<IInterceptedPropertyFinder>();
      _relatedMethodFinderMock = MockRepository.GenerateStrictMock<IRelatedMethodFinder>();

      _participant = new InterceptedDomainObjectParticipant (_typeDefinitionProviderMock, _interceptedPropertyFinderMock, _relatedMethodFinderMock);

      _proxyType = ProxyTypeObjectMother.Create (typeof (ConcreteBaseType));
    }

    [Test]
    public void ModifyType_AddMarkerInterface_OverridesHooks ()
    {
      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      var fakeProperties = Enumerable.Empty<Tuple<PropertyInfo, string>>();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (typeof (ConcreteBaseType))).Return (fakeDomainObjectType);
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
    public void ModifyType_InterceptsProperties ()
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

      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      var fakeProperties = new[] { Tuple.Create (property, "propertyIdentifier") };
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (typeof (ConcreteBaseType))).Return (fakeDomainObjectType);
      _interceptedPropertyFinderMock.Expect (mock => mock.GetProperties (fakeDomainObjectType)).Return (fakeProperties);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (getterBaseDefinition, _proxyType)).Return (getterOverride);
      _relatedMethodFinderMock.Expect (mock => mock.GetMostDerivedOverride (setterBaseDefinition, _proxyType)).Return (setterOverride);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsOverridable (getterOverride)).Return (true);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsAutomaticPropertyAccessor (getterOverride)).Return (true);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsOverridable (setterOverride)).Return (true);
      _interceptedPropertyFinderMock.Expect (mock => mock.IsAutomaticPropertyAccessor (setterOverride)).Return (true);

      _participant.ModifyType (_proxyType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      _interceptedPropertyFinderMock.VerifyAllExpectations();
      _relatedMethodFinderMock.VerifyAllExpectations();
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
      public string ReadOnlyProperty { get { return ""; } }
      public string WriteOnlyProperty { set { Dev.Null = value; } }
      protected string ProtectedProperty { get; set; }
    }

    // TODO 5370: 'ConcreteBaseType' not needed after TypePipe integration with re-mix.
    [IgnoreForMappingConfiguration]
    private class ConcreteBaseType : MyDomainObject
    {
      public override string SomeProperty { get; set; }
    }
  }
}