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
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class NewTypeStrategyTest
  {
    [Test]
    public void Initialization ()
    {
      var baseType = ReflectionObjectMother.GetSomeType();
      var attributes = TypeAttributes.Abstract;
      var interfaces = new Type[0];
      var fields = new FieldInfo[0];
      var constructors = new ConstructorInfo[0];

      var typeStrategy = new NewTypeStrategy (baseType, attributes, interfaces, fields, constructors);

      Assert.That (typeStrategy.GetBaseType (), Is.SameAs (baseType));
      Assert.That (typeStrategy.GetInterfaces (), Is.SameAs (interfaces));
      Assert.That (typeStrategy.GetFields (BindingFlags.Default), Is.SameAs (fields));
      Assert.That (typeStrategy.GetConstructors (BindingFlags.Default), Is.SameAs (constructors));
      Assert.That (typeStrategy.GetAttributeFlags (), Is.EqualTo (attributes));
    }

    [Test]
    public void GetUnderlyingSystemType ()
    {
      var typeStrategy = NewTypeStrategyObjectMother.Create();

      Assert.That (typeStrategy.GetUnderlyingSystemType(), Is.Null);
    }
  }
}