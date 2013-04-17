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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.TypePipe.UnitTesting.Expressions;
using Remotion.Development.TypePipe.UnitTesting.ObjectMothers.Expressions;
using Remotion.Development.TypePipe.UnitTesting.ObjectMothers.MutableReflection;
using Remotion.Development.TypePipe.UnitTesting.ObjectMothers.MutableReflection.Implementation;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using System.Linq;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class TargetTypeModifierTest
  {
    private IExpressionBuilder _expressionBuilderMock;
    private IAttributeGenerator _attributeGeneratorMock;

    private TargetTypeModifier _modifier;

    private Type _target;
    private MutableType _concreteTarget;
    private TargetTypeModifierContext _context;

    [SetUp]
    public void SetUp ()
    {
      _expressionBuilderMock = MockRepository.GenerateStrictMock<IExpressionBuilder>();
      _attributeGeneratorMock = MockRepository.GenerateStrictMock<IAttributeGenerator>();

      _modifier = new TargetTypeModifier (_expressionBuilderMock, _attributeGeneratorMock);

      _target = typeof (Target);
      _concreteTarget = new MutableTypeFactory().CreateProxy (_target);
      _context = new TargetTypeModifierContext (_target, _concreteTarget);
    }

    [Test]
    public void CreateContext ()
    {
      var result = _modifier.CreateContext (_target, _concreteTarget);

      Assert.That (result.Target, Is.SameAs (_target));
      Assert.That (result.ConcreteTarget, Is.SameAs (_concreteTarget));
    }

    [Test]
    public void ImplementInterfaces ()
    {
      var ifc = ReflectionObjectMother.GetSomeInterfaceType();

      _modifier.ImplementInterfaces (_context, new[] { ifc });

      Assert.That (_concreteTarget.AddedInterfaces, Is.EqualTo (new[] { ifc }));
    }

    [Test]
    public void AddFields ()
    {
      var nextCallProxyType = ReflectionObjectMother.GetSomeType();
      _attributeGeneratorMock
          .Expect (mock => mock.AddDebuggerBrowsableAttribute (Arg<MutableFieldInfo>.Is.Anything, Arg.Is (DebuggerBrowsableState.Never)))
          .Repeat.Times (4);

      _modifier.AddFields (_context, nextCallProxyType);

      _attributeGeneratorMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.AddedFields, Has.Count.EqualTo (4));
      var privateStaticAttributes = FieldAttributes.Private | FieldAttributes.Static;
      CheckField (_context.ClassContextField, "__classContext", typeof (ClassContext), privateStaticAttributes);
      CheckField (_context.MixinArrayInitializerField, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStaticAttributes);
      CheckField (_context.ExtensionsField, "__extensions", typeof (object[]), FieldAttributes.Private);
      CheckField (_context.FirstField, "__first", nextCallProxyType, FieldAttributes.Private);
    }

    [Test]
    public void AddTypeInitializations ()
    {
      _context.ClassContextField = ExpressionTreeObjectMother.GetSomeWritableExpression (typeof (ClassContext));
      _context.MixinArrayInitializerField = ExpressionTreeObjectMother.GetSomeWritableExpression (typeof (MixinArrayInitializer));
      var classContext = ClassContextObjectMother.Create();
      var concreteMixinType = ReflectionObjectMother.GetSomeType();
      var fakeClassContextExpression = ExpressionTreeObjectMother.GetSomeExpression (typeof (ClassContext));
      _expressionBuilderMock.Expect (mock => mock.CreateNewClassContextExpression (classContext)).Return (fakeClassContextExpression);

      _modifier.AddTypeInitializations (_context, classContext, new[] { concreteMixinType }.AsOneTime());

      _expressionBuilderMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.TypeInitializer, Is.Not.Null);
      var typeInitializations = _concreteTarget.MutableTypeInitializer.Body;

      var expectedTypeInitializations = Expression.Block (
          typeof (void),
          Expression.Assign (_context.ClassContextField, fakeClassContextExpression),
          Expression.Assign (
              _context.MixinArrayInitializerField,
              Expression.New (
                  typeof (MixinArrayInitializer).GetConstructors().Single(),
                  Expression.Constant (_target),
                  Expression.Constant (new[] { concreteMixinType }))));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedTypeInitializations, typeInitializations);
    }

    [Test]
    public void AddInitializations ()
    {
      _context.ExtensionsField = ExpressionTreeObjectMother.GetSomeExpression();
      var fakeInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionBuilderMock.Expect (mock => mock.CreateInitializationExpression (_concreteTarget, _context.ExtensionsField)).Return (fakeInitialization);

      _modifier.AddInitializations (_context);

      _expressionBuilderMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.Initializations, Is.EqualTo (new[] { fakeInitialization }));
    }

    [Test]
    public void ImplementIInitializableMixinTarget ()
    {
      var parameters = new[] { CustomParameterInfoObjectMother.Create (type: _concreteTarget), CustomParameterInfoObjectMother.Create (type: typeof (int)) };
      var nextCallProxyType = CustomTypeObjectMother.Create();
      var nextCallProxyTypeConstructor = CustomConstructorInfoObjectMother.Create (nextCallProxyType, parameters: parameters);
      ((TestableCustomType) nextCallProxyType).Constructors = new ConstructorInfo[] { nextCallProxyTypeConstructor };
      _context.MixinArrayInitializerField = ExpressionTreeObjectMother.GetSomeExpression (typeof (MixinArrayInitializer));
      _context.ExtensionsField = ExpressionTreeObjectMother.GetSomeWritableExpression (typeof (object[]));
      _context.FirstField = ExpressionTreeObjectMother.GetSomeWritableExpression (nextCallProxyType);
      _context.NextCallProxyConstructor = nextCallProxyTypeConstructor;
      _concreteTarget.AddInterface (typeof (IInitializableMixinTarget));
      var expectedMixinTypes = new[] { typeof (object), typeof (ClassImplementingIInitializableMixin) };

      _modifier.ImplementIInitializableMixinTarget (_context, expectedMixinTypes.AsOneTime());

      Assert.That (_concreteTarget.AddedMethods, Has.Count.EqualTo (2));
      var initializeMethod = _concreteTarget.AddedMethods.Single (m => m.Name.EndsWith ("Initialize"));
      var initializeAfterDeserializationMethod = _concreteTarget.AddedMethods.Single (m => m.Name.EndsWith ("InitializeAfterDeserialization"));

      var @this = new ThisExpression (_concreteTarget);
      var createMixinInstances = Expression.Assign (
          _context.ExtensionsField,
          Expression.Call (
              _context.MixinArrayInitializerField,
              "CreateMixinArray",
              Type.EmptyTypes,
              Expression.Property (Expression.Property (null, typeof (MixedObjectInstantiationScope), "Current"), "SuppliedMixinInstances")));
      var deserialization = Expression.Constant (false);
      var expectedInitializeBody = Expression.Block (
          Expression.Assign (
              _context.FirstField,
              Expression.New (_context.NextCallProxyConstructor, @this, Expression.Constant (0))),
          createMixinInstances,
          Expression.Block (
              Expression.Call (
                  Expression.Convert (
                      Expression.ArrayAccess (_context.ExtensionsField, Expression.Constant (1)),
                      typeof (IInitializableMixin)),
                  "Initialize",
                  Type.EmptyTypes,
                  @this,
                  Expression.New (_context.NextCallProxyConstructor, @this, Expression.Constant (2)),
                  deserialization)));

      var setMixinInstances = Expression.Block (
          Expression.Call (
              _context.MixinArrayInitializerField,
              "CheckMixinArray",
              Type.EmptyTypes,
              Expression.Parameter (typeof (object[]), "mixinInstances")),
          Expression.Assign (_context.ExtensionsField, Expression.Parameter (typeof (object[]), "mixinInstances")));
      var expectedInitializeAfterDeserializationBody = expectedInitializeBody.Replace (
          new Dictionary<Expression, Expression> { { createMixinInstances, setMixinInstances }, { deserialization, Expression.Constant (true) } });

      CheckExplicitMethodImplementation (
          initializeMethod, "Remotion.Mixins.CodeGeneration.DynamicProxy.IInitializableMixinTarget.Initialize", expectedInitializeBody);
      CheckExplicitMethodImplementation (
          initializeAfterDeserializationMethod,
          "Remotion.Mixins.CodeGeneration.DynamicProxy.IInitializableMixinTarget.InitializeAfterDeserialization",
          expectedInitializeAfterDeserializationBody);
    }

    [Test]
    public void ImplementIMixinTarget ()
    {
      _context.ClassContextField = ExpressionTreeObjectMother.GetSomeExpression (typeof (ClassContext));
      _context.ExtensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));
      _context.FirstField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      _concreteTarget.AddInterface (typeof (IMixinTarget));
      var fakeInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionBuilderMock.Expect (mock => mock.CreateInitializationExpression (_concreteTarget, _context.ExtensionsField)).Return (fakeInitialization);
      ExpectAddDebuggerDisplayAttribute (_attributeGeneratorMock, "Class context for " + _target.Name, "ClassContext");
      ExpectAddDebuggerDisplayAttribute (_attributeGeneratorMock, "Count = {__extensions.Length}", "Mixins");
      ExpectAddDebuggerDisplayAttribute (_attributeGeneratorMock, "Generated proxy", "FirstNextCallProxy");

      _modifier.ImplementIMixinTarget (_context);

      _expressionBuilderMock.VerifyAllExpectations();
      _attributeGeneratorMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.AddedProperties, Has.Count.EqualTo (3));
      var classContextProperty = _concreteTarget.AddedProperties.Single (p => p.Name.EndsWith ("ClassContext"));
      var mixinProperty = _concreteTarget.AddedProperties.Single (p => p.Name.EndsWith ("Mixins"));
      var firstNextCallProperty = _concreteTarget.AddedProperties.Single (p => p.Name.EndsWith ("FirstNextCallProxy"));

      CheckExplicitPropertyImplementation (
          classContextProperty, "Remotion.Mixins.IMixinTarget.ClassContext", _context.ClassContextField, Expression.Empty());
      CheckExplicitPropertyImplementation (
          mixinProperty, "Remotion.Mixins.IMixinTarget.Mixins", _context.ExtensionsField, fakeInitialization);
      CheckExplicitPropertyImplementation (
          firstNextCallProperty, "Remotion.Mixins.IMixinTarget.FirstNextCallProxy", _context.FirstField, fakeInitialization);
    }

    [Test]
    public void ImplementIntroducedInterfaces ([Values (MemberVisibility.Private, MemberVisibility.Public)] MemberVisibility visibility)
    {
      var receivedInterface = GetReceivedInterfacesForTargetWith<IntroducingMixin> (visibility);
      var methodIntroduciton = receivedInterface.IntroducedMethods.Single();
      var propertyIntroduction = receivedInterface.IntroducedProperties.Single();
      var eventIntroduction = receivedInterface.IntroducedEvents.Single();
      _context.ExtensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));

      Expression implementer = null;
      var modifierPartialMock = MockRepository.GeneratePartialMock<TargetTypeModifier> (_expressionBuilderMock, _attributeGeneratorMock);
      modifierPartialMock
          .Expect (
              mock => mock.ImplementIntroducedMethod (
                  Arg.Is (_concreteTarget),
                  Arg.Is (_context.ExtensionsField),
                  Arg<Expression>.Is.Anything,
                  Arg.Is (methodIntroduciton.InterfaceMember),
                  Arg.Is (methodIntroduciton.ImplementingMember),
                  Arg.Is (visibility)))
          .Return (null)
          .WhenCalled (mi => implementer = (Expression) mi.Arguments[2]);
      modifierPartialMock.Expect (mock => mock.ImplementIntroducedProperty (
          Arg.Is(_concreteTarget), Arg.Is(_context.ExtensionsField), Arg<Expression>.Matches (e => e == implementer), Arg.Is (propertyIntroduction)));
      modifierPartialMock.Expect (mock => mock.ImplementIntroducedEvent (
          Arg.Is(_concreteTarget), Arg.Is(_context.ExtensionsField), Arg<Expression>.Matches (e => e == implementer), Arg.Is (eventIntroduction)));

      modifierPartialMock.ImplementIntroducedInterfaces (_context, new[] { receivedInterface }.AsOneTime());

      modifierPartialMock.VerifyAllExpectations();
      var expectedImplementer = Expression.Convert (
          Expression.ArrayAccess (_context.ExtensionsField, Expression.Constant (1)), typeof (IIntroducedInterface));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedImplementer, implementer);
    }

    [TestCase (MemberVisibility.Public, "Method", MethodAttributes.Public | MethodAttributes.Virtual)]
    [TestCase (MemberVisibility.Private, "Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe.TargetTypeModifierTest.IIntroducedInterface.Method",
        MethodAttributes.Private | MethodAttributes.Virtual)]
    public void ImplementIntroducedMethod (MemberVisibility visibility, string expectedName, MethodAttributes expectedAttributes)
    {
      var extensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));
      var implementer = ExpressionTreeObjectMother.GetSomeExpression (typeof (IIntroducedInterface));
      var methodIntroduction = GetReceivedInterfacesForTargetWith<IntroducingMixin> (visibility).IntroducedMethods.Single();
      var interfaceMethod = methodIntroduction.InterfaceMember;
      var implementingMethod = methodIntroduction.ImplementingMember;
      _concreteTarget.AddInterface (typeof (IIntroducedInterface));
      var fakeInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionBuilderMock.Expect (mock => mock.CreateInitializationExpression (_concreteTarget, extensionsField)).Return (fakeInitialization);
      _attributeGeneratorMock
          .Expect (mock => mock.AddIntroducedMemberAttribute (Arg<MutableMethodInfo>.Is.Anything, Arg.Is (interfaceMethod), Arg.Is (implementingMethod)));
      _attributeGeneratorMock.Expect (mock => mock.ReplicateAttributes (Arg.Is (implementingMethod), Arg<MutableMethodInfo>.Is.Anything));

      var result = _modifier.ImplementIntroducedMethod (
          _concreteTarget, extensionsField, implementer, interfaceMethod, implementingMethod, visibility);

      _expressionBuilderMock.VerifyAllExpectations();
      _attributeGeneratorMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.AddedMethods, Is.EqualTo (new[] { result }));
      Assert.That (result.Name, Is.EqualTo (expectedName));
      Assert.That (result.Attributes.IsSet (expectedAttributes), Is.True);
      var expectedBody = Expression.Block (fakeInitialization, Expression.Call (implementer, interfaceMethod));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, result.Body);
    }

    [TestCase (MemberVisibility.Public, "ReadOnlyProperty")]
    [TestCase (MemberVisibility.Private, "Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe.TargetTypeModifierTest.IIntroducedInterface.ReadOnlyProperty")]
    public void ImplementIntroducedProperty_ReadOnlyProperty (MemberVisibility visibility, string expectedName)
    {
      var extensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));
      var implementer = ExpressionTreeObjectMother.GetSomeExpression (typeof (IIntroducedInterface));
      var propertyIntroduction = GetReceivedInterfacesForTargetWith<IntroducingMixin> (visibility).IntroducedProperties.Single();
      var interfaceProperty = propertyIntroduction.InterfaceMember;
      var implementingProperty = propertyIntroduction.ImplementingMember;
      _concreteTarget.AddInterface (typeof (IIntroducedInterface));
      var modifierPartialMock = MockRepository.GeneratePartialMock<TargetTypeModifier> (_expressionBuilderMock, _attributeGeneratorMock);
      var fakeGetMethod = MutableMethodInfoObjectMother.Create (_concreteTarget, returnType: typeof (int));
      modifierPartialMock
          .Expect (
              mock => mock.ImplementIntroducedMethod (
                  _concreteTarget, extensionsField, implementer, interfaceProperty.GetGetMethod(), implementingProperty.GetMethod, visibility))
          .Return (fakeGetMethod);
      _attributeGeneratorMock
          .Expect (mock => mock.AddIntroducedMemberAttribute (Arg<MutableMethodInfo>.Is.Anything, Arg.Is (interfaceProperty), Arg.Is (implementingProperty)));
      _attributeGeneratorMock.Expect (mock => mock.ReplicateAttributes (Arg.Is (implementingProperty), Arg<MutableMethodInfo>.Is.Anything));

      modifierPartialMock.ImplementIntroducedProperty (_concreteTarget, extensionsField, implementer, propertyIntroduction);

      modifierPartialMock.VerifyAllExpectations();
      _attributeGeneratorMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.AddedProperties, Has.Count.EqualTo (1));
      var addedProperty = _concreteTarget.AddedProperties.Single();
      Assert.That (addedProperty.Name, Is.EqualTo (expectedName));
      Assert.That (addedProperty.MutableGetMethod, Is.SameAs (fakeGetMethod));
      Assert.That (addedProperty.MutableSetMethod, Is.Null);
    }

    [TestCase (MemberVisibility.Public, "WriteOnlyProperty")]
    [TestCase (MemberVisibility.Private,
        "Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe.TargetTypeModifierTest.IIntroducedInterfaceWithWriteOnlyProperty.WriteOnlyProperty")]
    public void ImplementIntroducedProperty_WriteOnlyProperty (MemberVisibility visibility, string expectedName)
    {
      var extensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));
      var implementer = ExpressionTreeObjectMother.GetSomeExpression (typeof (IIntroducedInterface));
      var propertyIntroduction = GetReceivedInterfacesForTargetWith<IntroducingMixinWithWriteOnlyProperty> (visibility).IntroducedProperties.Single();
      var interfaceProperty = propertyIntroduction.InterfaceMember;
      var implementingProperty = propertyIntroduction.ImplementingMember;
      _concreteTarget.AddInterface (typeof (IIntroducedInterface));
      var modifierPartialMock = MockRepository.GeneratePartialMock<TargetTypeModifier> (_expressionBuilderMock, _attributeGeneratorMock);
      var fakeSetMethod = MutableMethodInfoObjectMother.Create (_concreteTarget, parameters: new[] { new ParameterDeclaration (typeof (int)) });
      modifierPartialMock
          .Expect (
              mock => mock.ImplementIntroducedMethod (
                  _concreteTarget, extensionsField, implementer, interfaceProperty.GetSetMethod(), implementingProperty.SetMethod, visibility))
          .Return (fakeSetMethod);
      _attributeGeneratorMock
          .Expect (mock => mock.AddIntroducedMemberAttribute (Arg<MutableMethodInfo>.Is.Anything, Arg.Is (interfaceProperty), Arg.Is (implementingProperty)));
      _attributeGeneratorMock.Expect (mock => mock.ReplicateAttributes (Arg.Is (implementingProperty), Arg<MutableMethodInfo>.Is.Anything));

      modifierPartialMock.ImplementIntroducedProperty (_concreteTarget, extensionsField, implementer, propertyIntroduction);

      modifierPartialMock.VerifyAllExpectations ();
      _attributeGeneratorMock.VerifyAllExpectations ();
      Assert.That (_concreteTarget.AddedProperties, Has.Count.EqualTo (1));
      var addedProperty = _concreteTarget.AddedProperties.Single ();
      Assert.That (addedProperty.Name, Is.EqualTo (expectedName));
      Assert.That (addedProperty.MutableGetMethod, Is.Null);
      Assert.That (addedProperty.MutableSetMethod, Is.SameAs (fakeSetMethod));
    }

    [TestCase (MemberVisibility.Public, "Event")]
    [TestCase (MemberVisibility.Private, "Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe.TargetTypeModifierTest.IIntroducedInterface.Event")]
    public void ImplementIntroducedEvent (MemberVisibility visibility, string expectedName)
    {
      var extensionsField = ExpressionTreeObjectMother.GetSomeExpression (typeof (object[]));
      var implementer = ExpressionTreeObjectMother.GetSomeExpression (typeof (IIntroducedInterface));
      var eventIntroduction = GetReceivedInterfacesForTargetWith<IntroducingMixin> (visibility).IntroducedEvents.Single ();
      var interfaceEvent = eventIntroduction.InterfaceMember;
      var implementingEvent = eventIntroduction.ImplementingMember;
      _concreteTarget.AddInterface (typeof (IIntroducedInterface));
      var modifierPartialMock = MockRepository.GeneratePartialMock<TargetTypeModifier> (_expressionBuilderMock, _attributeGeneratorMock);
      var fakeAddMethod = MutableMethodInfoObjectMother.Create (_concreteTarget, parameters: new[] { new ParameterDeclaration (typeof (Action)) });
      var fakeRemoveMethod = MutableMethodInfoObjectMother.Create (_concreteTarget, parameters: new[] { new ParameterDeclaration (typeof (Action)) });
      modifierPartialMock
          .Expect (
              mock => mock.ImplementIntroducedMethod (
                  _concreteTarget, extensionsField, implementer, interfaceEvent.GetAddMethod(), implementingEvent.AddMethod, visibility))
          .Return (fakeAddMethod);
      modifierPartialMock
          .Expect (
              mock => mock.ImplementIntroducedMethod (
                  _concreteTarget, extensionsField, implementer, interfaceEvent.GetRemoveMethod(), implementingEvent.RemoveMethod, visibility))
          .Return (fakeRemoveMethod);
      _attributeGeneratorMock
          .Expect (mock => mock.AddIntroducedMemberAttribute (Arg<MutableMethodInfo>.Is.Anything, Arg.Is (interfaceEvent), Arg.Is (implementingEvent)));
      _attributeGeneratorMock.Expect (mock => mock.ReplicateAttributes (Arg.Is (implementingEvent), Arg<MutableMethodInfo>.Is.Anything));

      modifierPartialMock.ImplementIntroducedEvent (_concreteTarget, extensionsField, implementer, eventIntroduction);

      modifierPartialMock.VerifyAllExpectations();
      _attributeGeneratorMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.AddedEvents, Has.Count.EqualTo (1));
      var addedEvent = _concreteTarget.AddedEvents.Single();
      Assert.That (addedEvent.Name, Is.EqualTo (expectedName));
      Assert.That (addedEvent.MutableAddMethod, Is.SameAs (fakeAddMethod));
      Assert.That (addedEvent.MutableRemoveMethod, Is.SameAs (fakeRemoveMethod));
    }

    private void CheckField (Expression fieldExpression, string expectedName, Type expectedType, FieldAttributes expectedAttributes)
    {
      var memberExpression = (MemberExpression) fieldExpression;
      var field = (FieldInfo) memberExpression.Member;

      if (field.IsStatic)
        Assert.That (memberExpression.Expression, Is.Null);
      else
        Assert.That (memberExpression.Expression, Is.TypeOf<ThisExpression>().And.Property ("Type").SameAs (_concreteTarget));

      Assert.That (field.DeclaringType, Is.SameAs (_concreteTarget));
      Assert.That (field.Name, Is.EqualTo (expectedName));
      Assert.That (field.FieldType, Is.SameAs (expectedType));
      Assert.That (field.Attributes, Is.EqualTo (expectedAttributes));
    }

    private void CheckExplicitMethodImplementation (MutableMethodInfo explicitOverride, string expectedName, Expression expectedBody)
    {
      Assert.That (explicitOverride.Name, Is.EqualTo (expectedName));
      Assert.That (explicitOverride.IsPrivate, Is.True);
      Assert.That (explicitOverride.IsVirtual, Is.True);
      Assert.That (explicitOverride.AddedExplicitBaseDefinitions, Has.Count.EqualTo (1));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, explicitOverride.Body);
    }

    private void CheckExplicitPropertyImplementation (
        MutablePropertyInfo property, string expectedName, Expression expectedBackingField, Expression expectedInitialization)
    {
      Assert.That (property.Name, Is.EqualTo (expectedName));
      Assert.That (property.Attributes, Is.EqualTo (PropertyAttributes.None));
      Assert.That (property.MutableSetMethod, Is.Null);
      Assert.That (property.MutableGetMethod, Is.Not.Null);

      var expectedGetMethodName = expectedName.Insert (expectedName.LastIndexOf ('.') + 1, "get_");
      var expectedGetMethodBody = Expression.Block (expectedInitialization, expectedBackingField);
      CheckExplicitMethodImplementation (property.MutableGetMethod, expectedGetMethodName, expectedGetMethodBody);
    }

    private void ExpectAddDebuggerDisplayAttribute (IAttributeGenerator attributeGeneratorMock, string debuggerDisplayString, string debuggerDisplayName)
    {
      attributeGeneratorMock.Expect (
          mock => mock.AddDebuggerDisplayAttribute (Arg<MutablePropertyInfo>.Is.Anything, Arg.Is (debuggerDisplayString), Arg.Is (debuggerDisplayName)));
    }

    private InterfaceIntroductionDefinition GetReceivedInterfacesForTargetWith<T> (MemberVisibility visibility)
    {
      var classContext = MixinConfiguration
          .BuildNew()
          .ForClass<Target>()
          .AddMixin<DummyMixin> () // Force introduction.Implementer.MixinIndex to be something other than '0'.
          .AddMixin<T>().WithIntroducedMemberVisibility (visibility)
          .BuildClassContext();
      var targetClassDefinition = TargetClassDefinitionFactory.CreateAndValidate (classContext);
      var receivedInterface = targetClassDefinition.ReceivedInterfaces.Single();

      return receivedInterface;
    }

    public interface IIntroducedInterface
    {
      void Method ();
      string ReadOnlyProperty { get;}
      event Action Event;
    }

    public class IntroducingMixin : IIntroducedInterface
    {
      public void Method () { throw new NotImplementedException(); }
      public string ReadOnlyProperty { get { throw new NotImplementedException(); } }
      public event Action Event;
    }

    public interface IIntroducedInterfaceWithWriteOnlyProperty
    {
      string WriteOnlyProperty { set; }
    }

    public class IntroducingMixinWithWriteOnlyProperty : IIntroducedInterfaceWithWriteOnlyProperty
    {
      public string WriteOnlyProperty { set { throw new NotImplementedException (); } }
    }

    public class Target { }

    public class DummyMixin { }

    private class ClassImplementingIInitializableMixin : IInitializableMixin
    {
      public void Initialize (object target, object next, bool deserialization)
      {
        throw new NotImplementedException();
      }
    }
  }
}