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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace TypePipe.IntegrationTests
{
  [TestFixture]
  public class ExplicitOverridesTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void OverrideExplicitly ()
    {
      var overriddenMethod = GetDeclaredMethod (typeof (A), "OverridableMethod");
      var type = AssembleType<B> (
          mutableType =>
          {
            var mutableMethodInfo = mutableType.AddMethod (
                "DifferentName",
                MethodAttributes.Private | MethodAttributes.Virtual,
                typeof (string),
                ParameterDeclaration.EmptyParameters,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.GetBaseCall (overriddenMethod), Expression.Constant (" explicitly overridden"));
                });
            mutableType.AddExplicitOverride (overriddenMethod, mutableMethodInfo);
            Assert.That (mutableType.AddedExplicitOverrides[overriddenMethod], Is.SameAs (mutableMethodInfo));
            Assert.That (mutableMethodInfo.BaseMethod, Is.Null);
            Assert.That (mutableMethodInfo.GetBaseDefinition(), Is.EqualTo (mutableMethodInfo));

            mutableMethodInfo.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  Assert.That (() => ctx.BaseMethod, Throws.TypeOf<NotSupportedException>());
                  return ctx.GetPreviousBody();
                });
          });

      A instance = (B) Activator.CreateInstance (type);
      var method = GetDeclaredMethod (type, "DifferentName");

      // Reflection doesn't handle explicit overrides in GetBaseDefinition.
      // If this changes, MutableMethodInfo.GetBaseDefinition() must be changed as well.
      Assert.That (method.GetBaseDefinition(), Is.EqualTo (method));

      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("A explicitly overridden"));
      Assert.That (instance.OverridableMethod(), Is.EqualTo ("A explicitly overridden"));
    }

    [Test]
    public void OverrideMultipleExplicitly ()
    {
      var overriddenMethod = GetDeclaredMethod (typeof (A), "OverridableMethod");
      var otherOverriddenMetod = GetDeclaredMethod (typeof (A), "OverridableMethodWithSameSignature");
      var type = AssembleType<B> (
          mutableType =>
          {
            var mutableMethodInfo = mutableType.AddMethod (
                "DifferentName",
                MethodAttributes.Private | MethodAttributes.Virtual,
                typeof (string),
                ParameterDeclaration.EmptyParameters,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.GetBaseCall (overriddenMethod), Expression.Constant (" explicitly overridden"));
                });
            mutableType.AddExplicitOverride (overriddenMethod, mutableMethodInfo);
            mutableType.AddExplicitOverride (otherOverriddenMetod, mutableMethodInfo);
            Assert.That (mutableType.AddedExplicitOverrides[overriddenMethod], Is.SameAs (mutableMethodInfo));
            Assert.That (mutableType.AddedExplicitOverrides[otherOverriddenMetod], Is.SameAs (mutableMethodInfo));
          });

      A instance = (B) Activator.CreateInstance (type);
      var method = GetDeclaredMethod (type, "DifferentName");

      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("A explicitly overridden"));
      Assert.That (instance.OverridableMethod (), Is.EqualTo ("A explicitly overridden"));
      Assert.That (instance.OverridableMethodWithSameSignature (), Is.EqualTo ("A explicitly overridden"));
    }

    [Test]
    [Ignore ("TODO 4813")]
    public void TurnExistingMethodIntoOverrideForBaseMethod ()
    {
      var overriddenMethod = GetDeclaredMethod (typeof (A), "OverridableMethod");
      var type = AssembleType<B> (
          mutableType =>
          {
            var overridingMethod = mutableType.ExistingMutableMethods.Single(m => m.Name == "UnrelatedMethod");
            mutableType.AddExplicitOverride (overriddenMethod, overridingMethod);
            Assert.That (mutableType.AddedExplicitOverrides[overriddenMethod], Is.SameAs (overridingMethod));
          });

      A instance = (B) Activator.CreateInstance (type);

      Assert.That (instance.OverridableMethod (), Is.EqualTo ("B unrelated"));
    }

    [Test]
    [Ignore ("TODO 4813")]
    public void OverridingWithMethodsFromBaseTypes_IsNotSupported ()
    {
      var overriddenMethod = GetDeclaredMethod (typeof (A), "OverridableMethod");
      var overridingMethod = GetDeclaredMethod (typeof (B), "UnrelatedMethod");

      AssembleType<C> (
          mutableType =>
          {
            Assert.That (
                () => mutableType.AddExplicitOverride (overriddenMethod, overridingMethod),
                Throws.ArgumentException.With.Message.EqualTo (
                    "The overriding method must be declared on this type.\r\nParameter name: overridingMethod"));
            Assert.That (mutableType.AddedExplicitOverrides, Is.Empty);
          });
    }

    [Test]
    public void OverrideShadowedMethod_AndShadowingMethod ()
    {
      var overriddenShadowedMethod = GetDeclaredMethod (typeof (A), "MethodShadowedByB");
      var overriddenShadowingMethod = GetDeclaredMethod (typeof (B), "MethodShadowedByB");

      var type = AssembleType<C> (
          mutableType =>
          {
            mutableType.AddMethod (
                "MethodShadowedByB",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof (string),
                ParameterDeclaration.EmptyParameters,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.True);
                  Assert.That (ctx.BaseMethod, Is.EqualTo (overriddenShadowingMethod));
                  return ExpressionHelper.StringConcat (ctx.GetBaseCall (ctx.BaseMethod), Expression.Constant (" implicitly overridden"));
                });
            var overrideForShadowedMethod = mutableType.AddMethod (
                "DifferentName",
                MethodAttributes.Private | MethodAttributes.Virtual,
                typeof (string),
                ParameterDeclaration.EmptyParameters,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.GetBaseCall (overriddenShadowedMethod), Expression.Constant (" explicitly overridden"));
                });
            mutableType.AddExplicitOverride (overriddenShadowedMethod, overrideForShadowedMethod);
          });

      var instance = (C) Activator.CreateInstance (type);

      Assert.That (instance.MethodShadowedByB (), Is.EqualTo ("B implicitly overridden"));
      Assert.That (((A) instance).MethodShadowedByB (), Is.EqualTo ("A explicitly overridden"));
    }

// ReSharper disable VirtualMemberNeverOverriden.Global
    public class A
    {
      public virtual string OverridableMethod ()
      {
        return "A";
      }

      public virtual string OverridableMethodWithSameSignature ()
      {
        return "A";
      }

      public virtual string MethodShadowedByB ()
      {
        return "A";
      }
    }

    public class B : A
    {
      public virtual string UnrelatedMethod ()
      {
        return "B unrelated";
      }

      public new virtual string MethodShadowedByB ()
      {
        return "B";
      }
    }

    public class C : B
    {
    }
// ReSharper restore VirtualMemberNeverOverriden.Global
  }
}