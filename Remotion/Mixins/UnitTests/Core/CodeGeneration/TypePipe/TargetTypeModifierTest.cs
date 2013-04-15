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
using Remotion.Development.UnitTesting;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class TargetTypeModifierTest
  {
    private IExpressionBuilder _expressionBuilderMock;

    private TargetTypeModifier _modifier;

    private Type _target;
    private MutableType _concreteTarget;
    private TargetTypeModifierContext _context;

    [SetUp]
    public void SetUp ()
    {
      _expressionBuilderMock = MockRepository.GenerateStrictMock<IExpressionBuilder>();

      _modifier = new TargetTypeModifier (_expressionBuilderMock);

      _target = ReflectionObjectMother.GetSomeSubclassableType();
      _concreteTarget = new MutableTypeFactory().CreateProxy (_target);
      _context = new TargetTypeModifierContext (_concreteTarget);
    }

    [Test]
    public void CreateContext ()
    {
      var result = _modifier.CreateContext (_concreteTarget);

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

      _modifier.AddFields (_context, nextCallProxyType);

      var privateStaticAttributes = FieldAttributes.Private | FieldAttributes.Static;
      CheckField (_context.ClassContextField, "__classContext", typeof (ClassContext), privateStaticAttributes);
      CheckField (_context.MixinArrayInitializerField, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStaticAttributes);
      CheckField (_context.ExtensionsField, "__extensions", typeof (object[]), FieldAttributes.Private);
      CheckField (_context.FirstField, "__first", nextCallProxyType, FieldAttributes.Private);

      var expctedFields = new[] { _context.ClassContextField, _context.MixinArrayInitializerField, _context.ExtensionsField, _context.FirstField };
      Assert.That (_concreteTarget.AddedFields, Is.EqualTo (expctedFields));
    }

    [Test]
    public void AddTypeInitializations ()
    {
      _context.ClassContextField = CustomFieldInfoObjectMother.Create (type: typeof (ClassContext), attributes: FieldAttributes.Static);
      _context.MixinArrayInitializerField = CustomFieldInfoObjectMother.Create (type: typeof (MixinArrayInitializer), attributes: FieldAttributes.Static);
      var targetType = ReflectionObjectMother.GetSomeType();
      var mixinType = ReflectionObjectMother.GetSomeOtherType();
      var composedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      var classContext = ClassContextObjectMother.Create (targetType, new[] { mixinType }, new[] { composedInterface });
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
                  Expression.Constant (classContext.Type),
                  Expression.Constant (new[] { concreteMixinType }))));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedTypeInitializations, typeInitializations);
    }

    [Test]
    public void AddInitializations ()
    {
      _context.ExtensionsField = MutableFieldInfoObjectMother.Create();
      var fakeInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionBuilderMock
          .Expect (mock => mock.CreateInitializationExpression (Arg<ThisExpression>.Is.Anything, Arg.Is (_context.ExtensionsField)))
          .WhenCalled (mi => Assert.That (mi.Arguments[0].As<ThisExpression>().Type, Is.SameAs (_concreteTarget)))
          .Return (fakeInitialization);

      _modifier.AddInitializations (_context);

      _expressionBuilderMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.Initializations, Is.EqualTo (new[] { fakeInitialization }));
    }

    [Test]
    public void ImplementIInitializableMixinTarget ()
    {
      var parameters = new[]{ CustomParameterInfoObjectMother.Create (type: _concreteTarget), CustomParameterInfoObjectMother.Create (type: typeof (int)) };
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
      var initializeMethod = _concreteTarget.AddedMethods.Single (m => m.Name == "Initialize");
      var initializeAfterDeserializationMethod = _concreteTarget.AddedMethods.Single (m => m.Name == "InitializeAfterDeserialization");

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
      ExpressionTreeComparer.CheckAreEqualTrees (expectedInitializeBody, initializeMethod.Body);

      var setMixinInstances = Expression.Block (
          Expression.Call (
              Expression.Field (null, _context.MixinArrayInitializerField),
              "CheckMixinArray",
              Type.EmptyTypes,
              Expression.Parameter (typeof (object[]), "mixinInstances")),
          Expression.Assign (Expression.Field (@this, _context.ExtensionsField), Expression.Parameter (typeof (object[]), "mixinInstances")));
      var initializeAfterDeserializationBody = expectedInitializeBody.Replace (
          new Dictionary<Expression, Expression> { { createMixinInstances, setMixinInstances }, { deserialization, Expression.Constant (true) } });
      ExpressionTreeComparer.CheckAreEqualTrees (initializeAfterDeserializationBody, initializeAfterDeserializationMethod.Body);
    }

    private void CheckField (FieldInfo field, string name, Type type, FieldAttributes attributes)
    {
      Assert.That (field.DeclaringType, Is.SameAs (_concreteTarget));
      Assert.That (field.Name, Is.EqualTo (name));
      Assert.That (field.FieldType, Is.SameAs (type));
      Assert.That (field.Attributes, Is.EqualTo (attributes));

      var debuggerBrowsableAttribute = ((MutableFieldInfo) field).AddedCustomAttributes.Single();
      Assert.That (debuggerBrowsableAttribute.Type, Is.SameAs (typeof (DebuggerBrowsableAttribute)));
      Assert.That (debuggerBrowsableAttribute.ConstructorArguments, Is.EqualTo (new[] { DebuggerBrowsableState.Never }));
      Assert.That (debuggerBrowsableAttribute.NamedArguments, Is.Empty);
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