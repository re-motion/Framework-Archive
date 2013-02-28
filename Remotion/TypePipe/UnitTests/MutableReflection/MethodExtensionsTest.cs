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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [Ignore]
  [TestFixture]
  public class MethodExtensionsTest
  {
    [Test]
    public void MakeTypePipeGenericMethod_MakesGenericMethodWithCustomTypeArgument ()
    {
      var genericMethodDefinition = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => Method<Dev.T, Dev.T>());
      var runtimeType = ReflectionObjectMother.GetSomeType();
      var customType = CustomTypeObjectMother.Create();

      var result = genericMethodDefinition.MakeTypePipeGenericMethod (runtimeType, customType);

      Assert.That (result.IsGenericMethod, Is.True);
      Assert.That (result.IsGenericMethodDefinition, Is.True);
      Assert.That (result.GetGenericArguments(), Is.EqualTo (new[] { runtimeType, customType }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "'ToString' is not a generic method definition. MakeTypePipeGenericMethod may only be called on a method for which"
        + " MethodInfo.IsGenericMethodDefinition is true.")]
    public void MakeTypePipeGenericMethod_NoGenericTypeDefinition ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => ToString());
      method.MakeTypePipeGenericMethod (typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The method has 2 generic parameter(s), but 1 generic argument(s) were provided. "
                          + "A generic argument must be provided for each generic parameter.\r\nParameter name: typeArguments")]
    public void MakeTypePipeGenericMethod_WrongNumberOfTypeArguments ()
    {
      var genericMethodDefinition = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => Method<Dev.T, Dev.T>());
      genericMethodDefinition.MakeTypePipeGenericMethod (typeof (int));
    }

    private void Method<T1, T2> () {}
  }
}