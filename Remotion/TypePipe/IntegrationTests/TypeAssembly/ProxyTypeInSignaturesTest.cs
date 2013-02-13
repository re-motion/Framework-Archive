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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class ProxyTypeInSignaturesTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void CustomAttributes ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            proxyType.AddCustomAttribute (CreateAttribute (proxyType));
            proxyType.AddField ("Field", typeof (int), FieldAttributes.Public).AddCustomAttribute (CreateAttribute (proxyType));
            var ctor = proxyType.AddConstructor (
                MethodAttributes.Public, new[] { new ParameterDeclaration (typeof (int), "p") }, ctx => ctx.CallThisConstructor());
            ctor.AddCustomAttribute (CreateAttribute (proxyType));
            ctor.MutableParameters.Single().AddCustomAttribute (CreateAttribute (proxyType));
            proxyType
                .AddMethod ("Method", MethodAttributes.Public, typeof (void), ParameterDeclaration.EmptyParameters, ctx => Expression.Empty())
                .AddCustomAttribute (CreateAttribute (proxyType));
            // TODO 4675: Add attribute to property
            // TODO 4676: Add attribute to event
          });

      var constructor = type.GetConstructor (new[] { typeof (int) });
      Assertion.IsNotNull (constructor);

      CheckCustomAttribute (type, type);
      CheckCustomAttribute (type.GetField ("Field"), type);
      CheckCustomAttribute (constructor, type);
      CheckCustomAttribute (constructor.GetParameters().Single(), type);
      CheckCustomAttribute (type.GetMethod ("Method"), type);
    }

    [Test]
    public void LocalVariable ()
    {
      var type = AssembleType<DomainType> (
          p => p.AddMethod (
              "Method",
              ctx =>
              {
                var localVariable = Expression.Parameter (p);
                return Expression.Block (new[] { localVariable }, Expression.Empty());
              }));

      var methodBody = type.GetMethod ("Method").GetMethodBody();
      Assertion.IsNotNull (methodBody);
      var localVariableType = methodBody.LocalVariables.Single().LocalType;

      Assert.That (localVariableType, Is.SameAs (type));
    }

    [Test]
    public void Field ()
    {
      var type = AssembleType<DomainType> (p => p.AddField ("Field", p, FieldAttributes.Public));

      var field = type.GetField ("Field");
      Assert.That (field.FieldType, Is.SameAs (type));
    }

    [Test]
    public void Constructor ()
    {
      var type = AssembleType<DomainType> (
          p => p.AddConstructor (MethodAttributes.Public, new[] { new ParameterDeclaration (p, "param") }, ctx => ctx.CallBaseConstructor()));

      var constructor = type.GetConstructor (new[] { type });
      Assertion.IsNotNull (constructor);
      Assert.That (constructor.GetParameters().Single().ParameterType, Is.SameAs (type));
    }

    [Test]
    public void Method ()
    {
      var type = AssembleType<DomainType> (
          p => p.AddMethod ("Method", MethodAttributes.Public, p, new[] { new ParameterDeclaration (p, "p") }, ctx => Expression.Default (p)));

      var method = type.GetMethod ("Method");
      Assert.That (method.ReturnType, Is.SameAs (type));
      Assert.That (method.GetParameters().Single().ParameterType, Is.SameAs (type));
    }

    [Test]
    public void GenericArgument ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            var genericTypeWithProxyTypeArgument = typeof (List<>).MakeTypePipeGenericType (proxyType);
            proxyType.AddMethod ("Method", ctx => Expression.Default (genericTypeWithProxyTypeArgument), returnType: genericTypeWithProxyTypeArgument);
          });

      var method = type.GetMethod ("Method");
      var expectedType = typeof (List<>).MakeGenericType (type);
      Assert.That (method.ReturnType, Is.SameAs (expectedType));
    }

    [Test]
    public void GenericArgument_Recursive ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            var funcType = typeof (Func<>).MakeTypePipeGenericType (proxyType);
            var enumerableType = typeof (IEnumerable<>).MakeTypePipeGenericType (funcType);
            proxyType.AddMethod ("Method", ctx => Expression.Default (enumerableType), returnType: enumerableType);
          });

      var method = type.GetMethod ("Method");
      var expectedType = typeof (IEnumerable<>).MakeGenericType (typeof (Func<>).MakeGenericType (type));
      Assert.That (method.ReturnType, Is.SameAs (expectedType));
    }

    private void CheckCustomAttribute (ICustomAttributeProvider customAttributeProvider, Type expectedType)
    {
      // Retrieving custom attribute data triggers type loading and assembly resolving.
      // The assembly cannot be resolved because it is not saved to disk.
      ResolveEventHandler resolver = (sender, args) => expectedType.Assembly;
      AppDomain.CurrentDomain.AssemblyResolve += resolver;
      try
      {
        var attribute = (AbcAttribute) customAttributeProvider.GetCustomAttributes (typeof (AbcAttribute), false).Single();
        Assert.That (attribute.Type, Is.SameAs (expectedType));
      }
      finally
      {
        AppDomain.CurrentDomain.AssemblyResolve -= resolver;
      }
    }

    private CustomAttributeDeclaration CreateAttribute (ProxyType proxyType)
    {
      var attributeCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new AbcAttribute (null));
      return new CustomAttributeDeclaration (attributeCtor, new object[] { proxyType });
    }

    public class DomainType { }

    public class AbcAttribute : Attribute
    {
      public readonly Type Type;
      public AbcAttribute (Type type) { Type = type; }
    }
  }
}