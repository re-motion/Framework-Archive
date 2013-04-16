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

      _target = ReflectionObjectMother.GetSomeSubclassableType();
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
          .Expect (mock => mock.AddDebuggerBrowsableAttribute (Arg<IMutableMember>.Is.Anything, Arg.Is (DebuggerBrowsableState.Never)))
          .Repeat.Times (4);

      _modifier.AddFields (_context, nextCallProxyType);

      _attributeGeneratorMock.VerifyAllExpectations();
      var expctedFields = new[] { _context.ClassContextField, _context.MixinArrayInitializerField, _context.ExtensionsField, _context.FirstField };
      Assert.That (_concreteTarget.AddedFields, Is.EqualTo (expctedFields));
      var classContextField = _concreteTarget.AddedFields.Single (f => f == _context.ClassContextField);
      var mixinArrayInitializerField = _concreteTarget.AddedFields.Single (f => f == _context.MixinArrayInitializerField);
      var extensionsField = _concreteTarget.AddedFields.Single (f => f == _context.ExtensionsField);
      var firstField = _concreteTarget.AddedFields.Single (f => f == _context.FirstField);

      var privateStaticAttributes = FieldAttributes.Private | FieldAttributes.Static;
      CheckField (classContextField, "__classContext", typeof (ClassContext), privateStaticAttributes);
      CheckField (mixinArrayInitializerField, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStaticAttributes);
      CheckField (extensionsField, "__extensions", typeof (object[]), FieldAttributes.Private);
      CheckField (firstField, "__first", nextCallProxyType, FieldAttributes.Private);
    }

    [Test]
    public void AddTypeInitializations ()
    {
      _context.ClassContextField = CustomFieldInfoObjectMother.Create (type: typeof (ClassContext), attributes: FieldAttributes.Static);
      _context.MixinArrayInitializerField = CustomFieldInfoObjectMother.Create (type: typeof (MixinArrayInitializer), attributes: FieldAttributes.Static);
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
          Expression.Assign (Expression.Field (null, _context.ClassContextField), fakeClassContextExpression),
          Expression.Assign (
              Expression.Field (null, _context.MixinArrayInitializerField),
              Expression.New (
                  typeof (MixinArrayInitializer).GetConstructors().Single(),
                  Expression.Constant (_target),
                  Expression.Constant (new[] { concreteMixinType }))));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedTypeInitializations, typeInitializations);
    }

    [Test]
    public void AddInitializations ()
    {
      _context.ExtensionsField = MutableFieldInfoObjectMother.Create();
      var fakeInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionBuilderMock.Expect (mock => mock.CreateInitializationExpression (_concreteTarget, _context.ExtensionsField)).Return (fakeInitialization);

      _modifier.AddInitializations (_context);

      _expressionBuilderMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.Initializations, Is.EqualTo (new[] { fakeInitialization }));
    }

    [Test]
    public void ImplementIInitializableMixinTarget ()
    {
      var parameters =
          new[] { CustomParameterInfoObjectMother.Create (type: _concreteTarget), CustomParameterInfoObjectMother.Create (type: typeof (int)) };
      var nextCallProxyType = CustomTypeObjectMother.Create();
      var nextCallProxyTypeConstructor = CustomConstructorInfoObjectMother.Create (nextCallProxyType, parameters: parameters);
      ((TestableCustomType) nextCallProxyType).Constructors = new ConstructorInfo[] { nextCallProxyTypeConstructor };
      _context.MixinArrayInitializerField = CustomFieldInfoObjectMother.Create (
          _concreteTarget, type: typeof (MixinArrayInitializer), attributes: FieldAttributes.Static);
      _context.FirstField = CustomFieldInfoObjectMother.Create (_concreteTarget, type: nextCallProxyType);
      _context.ExtensionsField = CustomFieldInfoObjectMother.Create (_concreteTarget, type: typeof (object[]));
      _context.NextCallProxyConstructor = nextCallProxyTypeConstructor;
      _concreteTarget.AddInterface (typeof (IInitializableMixinTarget));
      var expectedMixinTypes = new[] { typeof (object), typeof (ClassImplementingIInitializableMixin) };

      _modifier.ImplementIInitializableMixinTarget (_context, expectedMixinTypes.AsOneTime());

      Assert.That (_concreteTarget.AddedMethods, Has.Count.EqualTo (2));
      var initializeMethod = _concreteTarget.AddedMethods.Single (m => m.Name.EndsWith ("Initialize"));
      var initializeAfterDeserializationMethod = _concreteTarget.AddedMethods.Single (m => m.Name.EndsWith ("InitializeAfterDeserialization"));

      var @this = new ThisExpression (_concreteTarget);
      var createMixinInstances = Expression.Assign (
          Expression.Field (@this, _context.ExtensionsField),
          Expression.Call (
              Expression.Field (null, _context.MixinArrayInitializerField),
              "CreateMixinArray",
              Type.EmptyTypes,
              Expression.Property (Expression.Property (null, typeof (MixedObjectInstantiationScope), "Current"), "SuppliedMixinInstances")));
      var deserialization = Expression.Constant (false);
      var expectedInitializeBody = Expression.Block (
          Expression.Assign (
              Expression.Field (@this, _context.FirstField),
              Expression.New (_context.NextCallProxyConstructor, @this, Expression.Constant (0))),
          createMixinInstances,
          Expression.Block (
              Expression.Call (
                  Expression.Convert (
                      Expression.ArrayAccess (Expression.Field (@this, _context.ExtensionsField), Expression.Constant (1)),
                      typeof (IInitializableMixin)),
                  "Initialize",
                  Type.EmptyTypes,
                  @this,
                  Expression.New (_context.NextCallProxyConstructor, @this, Expression.Constant (2)),
                  deserialization)));

      var setMixinInstances = Expression.Block (
          Expression.Call (
              Expression.Field (null, _context.MixinArrayInitializerField),
              "CheckMixinArray",
              Type.EmptyTypes,
              Expression.Parameter (typeof (object[]), "mixinInstances")),
          Expression.Assign (Expression.Field (@this, _context.ExtensionsField), Expression.Parameter (typeof (object[]), "mixinInstances")));
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
      _context.ClassContextField = CustomFieldInfoObjectMother.Create (type: typeof (ClassContext), attributes: FieldAttributes.Static);
      _context.ExtensionsField = CustomFieldInfoObjectMother.Create (_concreteTarget, type: typeof (object[]));
      _context.FirstField = CustomFieldInfoObjectMother.Create (_concreteTarget, type: typeof (object));
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

    private void CheckField (MutableFieldInfo field, string expectedName, Type expectedType, FieldAttributes expectedAttributes)
    {
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
        MutablePropertyInfo property, string expectedName, FieldInfo expectedBackingField, Expression expectedInitialization)
    {
      Assert.That (property.Name, Is.EqualTo (expectedName));
      Assert.That (property.Attributes, Is.EqualTo (PropertyAttributes.None));
      Assert.That (property.MutableSetMethod, Is.Null);
      Assert.That (property.MutableGetMethod, Is.Not.Null);

      var expectedGetMethodName = expectedName.Insert (expectedName.LastIndexOf ('.') + 1, "get_");
      var instanceExpression = expectedBackingField.IsStatic ? null : new ThisExpression (_concreteTarget);
      var expectedGetMethodBody = Expression.Block (expectedInitialization, Expression.Field (instanceExpression, expectedBackingField));
      CheckExplicitMethodImplementation (property.MutableGetMethod, expectedGetMethodName, expectedGetMethodBody);
    }

    private void ExpectAddDebuggerDisplayAttribute (IAttributeGenerator attributeGeneratorMock, string debuggerDisplayString, string debuggerDisplayName)
    {
      attributeGeneratorMock.Expect (
          mock => mock.AddDebuggerDisplayAttribute (Arg<IMutableMember>.Is.Anything, Arg.Is (debuggerDisplayString), Arg.Is (debuggerDisplayName)));
    }

    private class ClassImplementingIInitializableMixin : IInitializableMixin
    {
      public void Initialize (object target, object next, bool deserialization)
      {
        throw new NotImplementedException();
      }
    }
  }
}