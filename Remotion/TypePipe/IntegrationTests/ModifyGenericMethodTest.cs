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
using System.Linq;
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace TypePipe.IntegrationTests
{
  [TestFixture]
  public class ModifyGenericMethodTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    [Ignore ("TODO 4789")]
    public void ExistingMethodWithGenericParameters ()
    {
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var mutableMethod = mutableType.ExistingMethods.Single (m => m.Name == "GenericMethod");
            Assert.That (mutableMethod.IsGenericMethod, Is.True);
            Assert.That (mutableMethod.IsGenericMethodDefinition, Is.True);
            var genericParameters = mutableMethod.GetGenericArguments ();
            var genericParameterNames = genericParameters.Select (t => t.Name);
            Assert.That (genericParameterNames, Is.EqualTo (new[] { "TKey", "TValue" }));

            mutableMethod.SetBody (
                ctx =>
                {
                  Assert.That (ctx.Parameters[0].Type, Is.SameAs (typeof (IDictionary<,>).MakeGenericType (genericParameters)));
                  var containsKeyMethod = ctx.Parameters[0].Type.GetMethod ("ContainsKey");
                  return Expression.Condition (
                      Expression.Call (ctx.Parameters[0], containsKeyMethod, ctx.Parameters[1]),
                      ctx.GetPreviousBody (),
                      Expression.Default (ctx.Parameters[1].Type));
                });
          });

      var method = type.GetMethod ("GenericMethod").MakeGenericMethod (typeof (int), typeof (string));
      var instance = (DomainType) Activator.CreateInstance (type);

      var dict = new Dictionary<int, string> { { 7, "seven" } };
      var result1 = method.Invoke (instance, new object[] { dict, 7 });
      var result2 = method.Invoke (instance, new object[] { dict, 8 });

      Assert.That (result1, Is.EqualTo ("seven"));
      Assert.That (result2, Is.EqualTo (null));
    }

    public class DomainType
    {
      public virtual TValue GenericMethod<TKey, TValue> (IDictionary<TKey, TValue> dict, TKey key)
        where TKey : IComparable<TKey>
        where TValue : class
      {
        return dict[key];
      }
    }
  }
}