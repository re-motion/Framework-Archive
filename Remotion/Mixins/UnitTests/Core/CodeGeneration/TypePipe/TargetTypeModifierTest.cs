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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.TypePipe.UnitTesting.Expressions;
using Remotion.Development.TypePipe.UnitTesting.ObjectMothers.Expressions;
using Remotion.Development.TypePipe.UnitTesting.ObjectMothers.MutableReflection;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
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
    private IComplexExpressionBuilder _complexExpressionBuilderMock;

    private TargetTypeModifier _modifier;

    private Type _target;
    private MutableType _concreteTarget;
    private TargetTypeModifierContext _context;

    [SetUp]
    public void SetUp ()
    {
      _complexExpressionBuilderMock = MockRepository.GenerateStrictMock<IComplexExpressionBuilder>();

      _modifier = new TargetTypeModifier (_complexExpressionBuilderMock);

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
      _context.ClassContextField = MutableFieldInfoObjectMother.Create (type: typeof (ClassContext), attributes: FieldAttributes.Static);
      _context.MixinArrayInitializerField = MutableFieldInfoObjectMother.Create (type: typeof (MixinArrayInitializer), attributes: FieldAttributes.Static);
      var targetType = ReflectionObjectMother.GetSomeType();
      var mixinType = ReflectionObjectMother.GetSomeOtherType();
      var composedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      var classContext = ClassContextObjectMother.Create (targetType, new[] { mixinType }, new[] { composedInterface });
      var concreteMixinType = ReflectionObjectMother.GetSomeType();
      var fakeClassContextExpression = ExpressionTreeObjectMother.GetSomeExpression (typeof (ClassContext));
      _complexExpressionBuilderMock.Expect (mock => mock.CreateNewClassContextExpression (classContext)).Return (fakeClassContextExpression);

      _modifier.AddTypeInitializations (_context, classContext, new[] { concreteMixinType }.AsOneTime());

      _complexExpressionBuilderMock.VerifyAllExpectations();
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
      _complexExpressionBuilderMock
          .Expect (mock => mock.CreateInitializationExpression (Arg<ThisExpression>.Is.Anything, Arg.Is (_context.ExtensionsField)))
          .WhenCalled (mi => Assert.That (mi.Arguments[0].As<ThisExpression>().Type, Is.SameAs (_concreteTarget)))
          .Return (fakeInitialization);

      _modifier.AddInitializations (_context);

      _complexExpressionBuilderMock.VerifyAllExpectations();
      Assert.That (_concreteTarget.Initializations, Is.EqualTo (new[] { fakeInitialization }));
    }

    private void CheckField (MutableFieldInfo field, string name, Type type, FieldAttributes attributes)
    {
      Assert.That (field.MutableDeclaringType, Is.SameAs (_concreteTarget));
      Assert.That (field.Name, Is.EqualTo (name));
      Assert.That (field.FieldType, Is.SameAs (type));
      Assert.That (field.Attributes, Is.EqualTo (attributes));
      Assert.That (field.AddedCustomAttributes, Has.Count.EqualTo (1));

      var debuggerBrowsableAttribute = field.AddedCustomAttributes.Single();
      Assert.That (debuggerBrowsableAttribute.Type, Is.SameAs (typeof (DebuggerBrowsableAttribute)));
      Assert.That (debuggerBrowsableAttribute.ConstructorArguments, Is.EqualTo (new[] { DebuggerBrowsableState.Never }));
      Assert.That (debuggerBrowsableAttribute.NamedArguments, Is.Empty);
    }
  }
}