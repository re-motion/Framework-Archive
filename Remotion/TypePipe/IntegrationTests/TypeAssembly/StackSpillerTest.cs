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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  public class StackSpillerTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void MaxStackSizeLargerThan8 ()
    {
      var methodWithManyArguments =
          NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.MethodWithManyArguments ("", "", "", "", "", "", "", "", ""));

      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var abcdefghi = methodWithManyArguments.GetParameters().Select ((p, i) => "" + (char) ('a' + i)).Select (Expression.Constant);
            mutableType
                .AllMutableMethods
                .Single (m => m.Name == "Method")
                .SetBody (ctx => Expression.Call (ctx.This, methodWithManyArguments, abcdefghi.Cast<Expression>().ToArray()));
          });

      var modifiedMethodBody = type.GetMethod ("Method").GetMethodBody();
      Assert.That (modifiedMethodBody.MaxStackSize, Is.EqualTo (10));
      // This reference + 9 arguments = 10

      var instance = (DomainType) Activator.CreateInstance (type);
      var result = instance.Method();

      Assert.That (result, Is.EqualTo ("abcdefghi"));
    }

    public class DomainType
    {
      public string MethodWithManyArguments (string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9)
      {
        return s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9;
      }

      public virtual string Method ()
      {
        return "";
      }
    }
  }
}