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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class MethodInstantiationInfoTest
  {
    private MethodInfo _genericMethodDefinition;

    private Type _customType;
    private Type _runtimeType;

    private MethodInstantiationInfo _info1;
    private MethodInstantiationInfo _info2;

    [SetUp]
    public void SetUp ()
    {
      _genericMethodDefinition = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => GenericMethod<Dev.T> (null));

      _customType = CustomTypeObjectMother.Create();
      _runtimeType = ReflectionObjectMother.GetSomeType();

      _info1 = new MethodInstantiationInfo (_genericMethodDefinition, new[] { _customType }.AsOneTime());
      _info2 = new MethodInstantiationInfo (_genericMethodDefinition, new[] { _runtimeType });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_info1.GenericMethodDefinition, Is.SameAs (_genericMethodDefinition));
      Assert.That (_info1.TypeArguments, Is.EqualTo (new[] { _customType }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Specified method must be a generic method definition.\r\nParameter name: genericMethodDefinition")]
    public void Initialization_NoGenericMethodDefinition ()
    {
      var method = ReflectionObjectMother.GetSomeNonGenericMethod();
      Dev.Null = new MethodInstantiationInfo (method, Type.EmptyTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Generic parameter count of the generic method definition does not match the number of supplied type arguments.\r\nParameter name: typeArguments")]
    public void Initialization_NonMatchingGenericArgumentCount ()
    {
      Dev.Null = new MethodInstantiationInfo (_genericMethodDefinition, Type.EmptyTypes);
    }

    void GenericMethod<T> (T t) {}
  }
}