// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class TypeModificationHandlerTest
  {
    private IExpressionPreparer _expressionPreparerMock;
    private ITypeBuilder _subclassProxyBuilderMock;
    private IILGeneratorFactory _ilGeneratorFactoryStub;
    private DebugInfoGenerator _debugInfoGeneratorStub;
    private ReflectionToBuilderMap _reflectionToBuilderMap;

    private TypeModificationHandler _handler;

    [SetUp]
    public void SetUp ()
    {
      _expressionPreparerMock = MockRepository.GenerateStrictMock<IExpressionPreparer>();
      _subclassProxyBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      _ilGeneratorFactoryStub = MockRepository.GenerateStub<IILGeneratorFactory>();
      _debugInfoGeneratorStub = MockRepository.GenerateStub<DebugInfoGenerator>();
      _reflectionToBuilderMap = new ReflectionToBuilderMap();

      _handler = new TypeModificationHandler (
          _subclassProxyBuilderMock, _expressionPreparerMock, _reflectionToBuilderMap, _ilGeneratorFactoryStub, _debugInfoGeneratorStub);
    }

    [Test]
    public void Initialization_NullDebugInfoGenerator ()
    {
      var handler = new TypeModificationHandler (
          _subclassProxyBuilderMock, _expressionPreparerMock, _reflectionToBuilderMap, _ilGeneratorFactoryStub, null);
      Assert.That (handler.DebugInfoGenerator, Is.Null);
    }

    [Test]
    public void HandleAddedInterface ()
    {
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      _subclassProxyBuilderMock.Expect (mock => mock.AddInterfaceImplementation (addedInterface));

      _handler.HandleAddedInterface (addedInterface);

      _subclassProxyBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleAddedField ()
    {
      var addedField = MutableFieldInfoObjectMother.Create();
      var fieldBuilderStub = MockRepository.GenerateStub<IFieldBuilder>();

      _subclassProxyBuilderMock
          .Expect(mock => mock.DefineField (addedField.Name, addedField.FieldType, addedField.Attributes))
          .Return (fieldBuilderStub);

      _handler.HandleAddedField (addedField);

      _subclassProxyBuilderMock.VerifyAllExpectations ();
      Assert.That (_reflectionToBuilderMap.GetBuilder (addedField), Is.SameAs (fieldBuilderStub));
    }

    [Test]
    public void HandleAddedField_WithCustomAttribute ()
    {
      var constructor = ReflectionObjectMother.GetConstructor (() => new CustomAttribute(""));
      var property = ReflectionObjectMother.GetProperty ((CustomAttribute attr) => attr.Property);
      var field = ReflectionObjectMother.GetField ((CustomAttribute attr) => attr.Field);
      var constructorArguments = new object[] { "ctorArgs" };
      var declaration = new CustomAttributeDeclaration (
          constructor,
          constructorArguments,
          new NamedAttributeArgumentDeclaration (property, 7),
          new NamedAttributeArgumentDeclaration (field, "test"));
      var addedField = MutableFieldInfoObjectMother.Create ();
      addedField.AddCustomAttribute (declaration);

      var fieldBuilderMock = MockRepository.GenerateMock<IFieldBuilder>();
      _subclassProxyBuilderMock
          .Stub (stub => stub.DefineField (addedField.Name, addedField.FieldType, addedField.Attributes))
          .Return (fieldBuilderMock);
      fieldBuilderMock
          .Expect (mock => mock.SetCustomAttribute (Arg<CustomAttributeBuilder>.Is.Anything))
          .WhenCalled (mi => CheckCustomAttributeBuilder (
              (CustomAttributeBuilder) mi.Arguments[0], 
              constructor,
              constructorArguments,
              new[] { property },
              new object[]  { 7 },
              new[] { field },
              new[] { "test" }));
      
      _handler.HandleAddedField (addedField);

      fieldBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleAddedConstructor_CallsAddConstructorToSubclassProxy ()
    {
      var addedCtorDescriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew();

      CheckThatMethodIsDelegatedToAddConstructorToSubclassProxy (_handler.HandleAddedConstructor, addedCtorDescriptor);
    }

    [Test]
    public void CloneExistingConstructor_CallsAddConstructorToSubclassProxy ()
    {
      var originalCtor = ReflectionObjectMother.GetSomeConstructor();
      var existingCtorDescriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting (originalCtor);

      CheckThatMethodIsDelegatedToAddConstructorToSubclassProxy (_handler.CloneExistingConstructor, existingCtorDescriptor);
    }

    [Test]
    public void AddConstructorToSubclassProxy ()
    {
      var parameterDeclarations =
          new[]
          {
              ParameterDeclarationObjectMother.Create (typeof (string), "p1", ParameterAttributes.In),
              ParameterDeclarationObjectMother.Create (typeof (int).MakeByRefType(), "p2", ParameterAttributes.Out)
          };
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (parameterDeclarations: parameterDeclarations);
      var mutableConstructor = MutableConstructorInfoObjectMother.Create (underlyingConstructorInfoDescriptor: descriptor);

      var expectedAttributes = mutableConstructor.Attributes;
      var expectedCallingConvention = mutableConstructor.CallingConvention;
      var expectedParameterTypes = new[] { typeof (string), typeof (int).MakeByRefType() };
      var constructorBuilderMock = MockRepository.GenerateStrictMock<IConstructorBuilder> ();
      _subclassProxyBuilderMock
          .Expect (mock => mock.DefineConstructor (expectedAttributes, expectedCallingConvention, expectedParameterTypes))
          .Return (constructorBuilderMock);

      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionPreparerMock.Expect (mock => mock.PrepareConstructorBody (mutableConstructor)).Return (fakeBody);

      constructorBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.In, "p1"));
      constructorBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "p2"));

      constructorBuilderMock
          .Expect (mock => mock.SetBody (Arg<LambdaExpression>.Is.Anything, Arg.Is (_ilGeneratorFactoryStub), Arg.Is (_debugInfoGeneratorStub)))
          .WhenCalled (mi =>
          {
            var lambdaExpression = (LambdaExpression) mi.Arguments[0];
            Assert.That (lambdaExpression.Body, Is.AssignableTo<BlockExpression> ());
            var blockExpression = (BlockExpression) lambdaExpression.Body;
            Assert.That (blockExpression.Expressions, Is.EqualTo (new[] { fakeBody }));
            Assert.That (blockExpression.Type, Is.SameAs (typeof (void)));
            Assert.That (lambdaExpression.Parameters, Is.EqualTo (mutableConstructor.ParameterExpressions));
          });

      CallAddConstructorToSubclassProxy (_handler, mutableConstructor);

      _subclassProxyBuilderMock.VerifyAllExpectations();
      _expressionPreparerMock.VerifyAllExpectations();
      constructorBuilderMock.VerifyAllExpectations();

      Assert.That (_reflectionToBuilderMap.GetBuilder (mutableConstructor), Is.SameAs (constructorBuilderMock));
    }

    [Test]
    public void AddConstructorToSubclassProxy_WithByRefParameters ()
    {
      var byRefType = typeof (object).MakeByRefType();
      
      var parameterDeclarations = new[] { ParameterDeclarationObjectMother.Create (byRefType) };
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (parameterDeclarations: parameterDeclarations);
      var mutableConstructor = MutableConstructorInfoObjectMother.Create (underlyingConstructorInfoDescriptor: descriptor);

      var constructorBuilderStub = MockRepository.GenerateStub<IConstructorBuilder> ();
      _subclassProxyBuilderMock
          .Expect (
              mock => mock.DefineConstructor (
                  Arg<MethodAttributes>.Is.Anything, Arg<CallingConventions>.Is.Anything, Arg<Type[]>.List.Equal (new[] { byRefType })))
          .Return (constructorBuilderStub);

      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression ();
      _expressionPreparerMock.Stub (stub => stub.PrepareConstructorBody (mutableConstructor)).Return (fakeBody);

      CallAddConstructorToSubclassProxy (_handler, mutableConstructor);

      _subclassProxyBuilderMock.VerifyAllExpectations ();
    }

    private void CheckCustomAttributeBuilder (
        CustomAttributeBuilder builder,
        ConstructorInfo expectedCtor,
        object[] expectedCtorArgs,
        PropertyInfo[] expectedPropertyInfos,
        object[] expectedPropertyValues,
        FieldInfo[] expectedFieldInfos,
        object[] expectedFieldValues)
    {
      var actualConstructor = (ConstructorInfo) PrivateInvoke.GetNonPublicField (builder, "m_con");
      var actualConstructorArgs = (object[]) PrivateInvoke.GetNonPublicField (builder, "m_constructorArgs");
      var actualBlob = (byte[]) PrivateInvoke.GetNonPublicField (builder, "m_blob");

      Assert.That (actualConstructor, Is.SameAs (expectedCtor));
      Assert.That (actualConstructorArgs, Is.EqualTo (expectedCtorArgs));

      var testBuilder = new CustomAttributeBuilder (
          expectedCtor, expectedCtorArgs, expectedPropertyInfos, expectedPropertyValues, expectedFieldInfos, expectedFieldValues);
      var expectedBlob = (byte[]) PrivateInvoke.GetNonPublicField (testBuilder, "m_blob");
      Assert.That (actualBlob, Is.EqualTo (expectedBlob));
    }

    private void CallAddConstructorToSubclassProxy (TypeModificationHandler handler, MutableConstructorInfo mutableConstructor)
    {
      PrivateInvoke.InvokeNonPublicMethod (handler, "AddConstructorToSubclassProxy", mutableConstructor);
    }

    private void CheckThatMethodIsDelegatedToAddConstructorToSubclassProxy (
        Action<MutableConstructorInfo> methodInvocation,
        UnderlyingConstructorInfoDescriptor descriptor)
    {
      var mutableConstructor = MutableConstructorInfoObjectMother.Create (underlyingConstructorInfoDescriptor: descriptor);

      var constructorBuilderStub = MockRepository.GenerateStub<IConstructorBuilder> ();
      _subclassProxyBuilderMock
          .Expect (mock => mock.DefineConstructor (Arg<MethodAttributes>.Is.Anything, Arg<CallingConventions>.Is.Anything, Arg<Type[]>.Is.Anything))
          .Return (constructorBuilderStub);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression ();
      _expressionPreparerMock.Expect (mock => mock.PrepareConstructorBody (mutableConstructor)).Return (fakeBody);

      methodInvocation (mutableConstructor);

      Assert.That (_reflectionToBuilderMap.GetBuilder (mutableConstructor), Is.SameAs (constructorBuilderStub));
    }

    public class CustomAttribute : Attribute
    {
// ReSharper disable UnassignedField.Global
      public string Field;
// ReSharper restore UnassignedField.Global

      public CustomAttribute (string ctorArgument)
      {
        CtorArgument = ctorArgument;

        Dev.Null = CtorArgument;
        Dev.Null = Property;
        Property = 0;
      }

      public string CtorArgument { get; private set; }
      public int Property { get; set; }
    }
  }
}