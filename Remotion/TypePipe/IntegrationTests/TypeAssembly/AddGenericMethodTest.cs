﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class AddGenericMethodTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void GenericParameterInSignature_AndLocalVariable ()
    {
      var type = AssembleType<DomainType> (
          p => p.AddGenericMethod (
              "GenericMethod",
              MethodAttributes.Public,
              new[] { new GenericParameterDeclaration ("T", GenericParameterAttributes.NotNullableValueTypeConstraint) },
              returnTypeProvider: ctx => ctx.GenericParameters[0],
              parameterProvider: ctx => new[] { new ParameterDeclaration (ctx.GenericParameters[0], "arg") },
              bodyProvider: ctx =>
              {
                var local = Expression.Variable (ctx.GenericParameters[0]);
                return Expression.Block (
                    new[] { local },
                    Expression.Assign (local, ctx.Parameters[0]));
              }));

      var genericMethod = type.GetMethod ("GenericMethod");
      Assert.That (genericMethod.IsGenericMethodDefinition, Is.True);
      var genericParameter = genericMethod.GetGenericArguments().Single();
      Assert.That (genericParameter.Name, Is.EqualTo ("T"));
      Assert.That (genericParameter.GenericParameterAttributes, Is.EqualTo (GenericParameterAttributes.NotNullableValueTypeConstraint));

      var method = genericMethod.MakeGenericMethod (typeof (long));
      var instance = Activator.CreateInstance (type);
      var result = method.Invoke (instance, new object[] { 5L });
      Assert.That (result, Is.EqualTo (5L));
    }

    [Test]
    public void GenericMethodParametersUsedInsideParameters_AndInvokingLambda ()
    {
      var type = AssembleType<DomainType> (
          p => p.AddGenericMethod (
              "GenericMethod",
              MethodAttributes.Public | MethodAttributes.Static,
              new[] { new GenericParameterDeclaration ("TArg"), new GenericParameterDeclaration ("TReturn") },
              ctx => ctx.GenericParameters[1],
              ctx =>
              new[]
              {
                  new ParameterDeclaration (typeof (Func<,>).MakeTypePipeGenericType (ctx.GenericParameters[0], ctx.GenericParameters[1]), "conv"),
                  new ParameterDeclaration (ctx.GenericParameters[0], "arg")
              },
              ctx => Expression.Invoke (ctx.Parameters[0], ctx.Parameters[1])));

      var method = type.GetMethod ("GenericMethod").MakeGenericMethod (typeof (int), typeof (string));
      Assert.That (method.IsStatic, Is.True);
      Func<int, string> converter = i => "Integer " + i;
      var result = method.Invoke (null, new object[] { converter, 7 });
      Assert.That (result, Is.EqualTo ("Integer 7"));
    }

    [Test]
    public void Constraints_ReferenceTypes_AndBaseTypeAndInterfaces ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface o) => o.GetTypeName());

      var type = AssembleType<DomainType> (
          p => p.AddGenericMethod (
              "GenericMethod",
              MethodAttributes.Public,
              new[]
              {
                  new GenericParameterDeclaration (
                      "T",
                      GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint,
                      ctx => new[] { typeof (BaseType), typeof (IDomainInterface) })
              },
              ctx => typeof (string),
              ctx => new[] { new ParameterDeclaration (ctx.GenericParameters[0], "arg") },
              ctx => Expression.Call (Expression.New (ctx.GenericParameters[0]), interfaceMethod)));

      var genericMethod = type.GetMethod ("GenericMethod");
      var genericParameter = genericMethod.GetGenericArguments().Single();
      Assert.That (
          genericParameter.GenericParameterAttributes,
          Is.EqualTo (GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint));
      Assert.That (genericParameter.GetGenericParameterConstraints(), Is.EquivalentTo (new[] { typeof (BaseType), typeof (IDomainInterface) }));
      Assert.That (genericParameter.BaseType, Is.SameAs (typeof (BaseType)));
      Assert.That (genericParameter.GetInterfaces(), Is.EqualTo (new[] { typeof (IDomainInterface) }));

      var method = genericMethod.MakeGenericMethod (typeof (DomainType));
      var instance = Activator.CreateInstance (type);
      var result = method.Invoke (instance, new object[] { null });
      Assert.That (result, Is.EqualTo ("DomainType"));
    }

    [Test]
    public void Constraints_ValueTypes_ConstraintContainingGenericMethodParameter ()
    {
      // public bool GenericEquals<T> (T a, T b)
      //    where T : IComparable<>
      // {  return a.CompareTo(b) == 0; }

      var type = AssembleType<DomainType> (
          p => p.AddGenericMethod (
              "GenericEquals",
              MethodAttributes.Public,
              new[]
              {
                  new GenericParameterDeclaration (
                      "T",
                      GenericParameterAttributes.NotNullableValueTypeConstraint,
                      constraintProvider: ctx => new[] { typeof (IComparable<>).MakeTypePipeGenericType (ctx.GenericParameters[0]) })
              },
              ctx => typeof (bool),
              ctx => new[] { new ParameterDeclaration (ctx.GenericParameters[0], "a"), new ParameterDeclaration (ctx.GenericParameters[0], "b") },
              ctx =>
              {
                var compareMethod = typeof(IComparable<>).MakeTypePipeGenericType(ctx.GenericParameters[0]).GetMethod ("CompareTo");
                return Expression.Equal (Expression.Call (ctx.Parameters[0], compareMethod, ctx.Parameters[1]), Expression.Constant (0));
              }));

      var genericMethod = type.GetMethod ("GenericEquals");
      var genericParameter = genericMethod.GetGenericArguments().Single();
      Assert.That (genericParameter.IsGenericParameter, Is.True);
      Assert.That (
          genericParameter.GetGenericParameterConstraints(),
          Is.EquivalentTo (new[] { typeof (ValueType), typeof (IComparable<>).MakeGenericType (genericParameter) }));
      var parameterTypes = genericMethod.GetParameters().Select (p => p.ParameterType).ToList();
      Assert.That (parameterTypes[0], Is.SameAs (parameterTypes[1]).And.SameAs (genericParameter));

      var method = genericMethod.MakeGenericMethod (typeof (int));
      var instance = Activator.CreateInstance (type);

      Assert.That (method.Invoke (instance, new object[] { 7, 7 }), Is.True);
      Assert.That (method.Invoke (instance, new object[] { 7, 8 }), Is.False);
    }

    // TODO: test for passing a 1) value type and afterwards 2) a reference type to the same generic method, as a parameter which is has a type of
    // generic parameter (defined on the method) and is constrained via an interface interface.

    public interface IDomainInterface
    {
      string GetTypeName ();
    }
    public class BaseType { }
    public class DomainType : BaseType, IDomainInterface
    {
      public string GetTypeName () { return GetType().Name; }
    }
  }
}