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
  // [Ignore ("4703")]
  public class SetConstructorBodyTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void ModifyExistingConstructor ()
    {
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var existingConstructor = mutableType.ExistingConstructors.Single();
            var concatMethod = typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) });
            existingConstructor.SetBody (
                ctx => Expression.Block (
                    ctx.GetPreviousBody (Expression.Add (ctx.Parameters[0], Expression.Constant (" cd"), concatMethod)),
                    // TODO 4744: Use Expression.Property (ctx.This, "SettableProperty")
                    Expression.Assign (Expression.Property (ctx.This, typeof (DomainType).GetProperty ("SettableProperty")), ctx.Parameters[0])));
          });

      var instance = (DomainType) Activator.CreateInstance (type, "ab");

      Assert.That (instance.CtorArgument, Is.EqualTo ("ab cd"));
      Assert.That (instance.SettableProperty, Is.EqualTo ("ab"));
    }

    [Test]
    public void ModifyAddedConstructor ()
    {
      var type = AssembleType<DomainType> (
          mutableType => mutableType.AddConstructor (
              MethodAttributes.Public,
              new ParameterDeclaration[0],
              ctx => ctx.GetConstructorCall (Expression.Constant ("added"))),
          mutableType =>
          {
            var addedCtor = mutableType.GetConstructor (Type.EmptyTypes);
            var mutableCtor = mutableType.GetMutableConstructor (addedCtor);
            mutableCtor.SetBody (ctx => ctx.GetConstructorCall (Expression.Constant ("modified added")));
          });

      var instance = (DomainType) Activator.CreateInstance (type);

      Assert.That (instance.CtorArgument, Is.EqualTo ("modified added"));
    }

    [Test]
    public void DelegateToExistingWhichIsChanged ()
    {
      var type = AssembleType<DomainType> (
          mutableType => mutableType.AddConstructor (
              MethodAttributes.Public,
              new ParameterDeclaration[0],
              ctx => ctx.GetConstructorCall (Expression.Constant ("added"))),
          mutableType =>
          {
            var existingCtor = typeof (DomainType).GetConstructor (new[] { typeof (string) });
            var mutableCtor = mutableType.GetMutableConstructor (existingCtor);
             mutableCtor.SetBody (ctx => ctx.GetPreviousBody (Expression.Constant ("modified existing")));
          });

      var instance = (DomainType) Activator.CreateInstance (type);

      Assert.That (instance.CtorArgument, Is.EqualTo ("modified existing"));
    }

    public class DomainType
    {
      public DomainType (string ctorArgument)
      {
        CtorArgument = ctorArgument;
      }

      public string CtorArgument { get; private set; }
      public string SettableProperty { get; set; }
    }
  }
}