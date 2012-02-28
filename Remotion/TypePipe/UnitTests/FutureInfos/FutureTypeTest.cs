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
using System.Reflection.Emit;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.FutureInfos;

namespace Remotion.TypePipe.UnitTests.FutureInfos
{
  [TestFixture]
  public class FutureTypeTest
  {
    private ModuleBuilder _moduleBuilder;

    [TestFixtureSetUp]
    public void GenerateAssembly ()
    {
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName ("FutureTypeTest"), AssemblyBuilderAccess.RunAndSave);
      _moduleBuilder = assemblyBuilder.DefineDynamicModule ("FutureTypeTest.dll");
    }

    //[Test]
    //public void Initialization ()
    //{
    //  // TODO
    //}

    [Test]
    public void FutureTypeIsAType ()
    {
      Assert.That (new FutureType(), Is.InstanceOf<Type> ());
      Assert.That (new FutureType(), Is.AssignableTo<Type> ());
    }

    [Test]
    public void BaseType ()
    {
      Assert.That (new FutureType().BaseType, Is.EqualTo (typeof (object)));
    }

    [Test]
    public void GetConstructor ()
    {
      // Arrange
      var futureType = new FutureType();

      BindingFlags bindingFlags = (BindingFlags) (-1);  // Does not matter
      Binder binder = null;                             // Does not matter
      Type[] parameterTypes = Type.EmptyTypes;          // Does not matter, cannot be null
      ParameterModifier[] parameterModifiers = null;    // Does not matter

      // Act
      var constructor = futureType.GetConstructor (bindingFlags, binder, parameterTypes, parameterModifiers);

      // Assert
      Assert.That (constructor, Is.TypeOf<FutureConstructor>());
    }

    [Test]
    public void SetTypeBuilder_ThrowsIfCalledMoreThanOnce ()
    {
      // Arrange
      var futureType = new FutureType ();
      var typeBuilder = CreateTypeBuilder ("SetTypeBuilder_ThrowsIfCalledMoreThanOnce");

      // Act
      TestDelegate action = () => futureType.SetTypeBuilder (typeBuilder);

      // Assert
      Assert.That (action, Throws.Nothing);
      Assert.That (action, Throws.InvalidOperationException
        .With.Message.EqualTo ("TypeBuilder already set"));
    }

    //// TODO: Maybe move this test into separate integration test file?
    //[Ignore ("Implement finer tests first")]
    [Test]
    public void FutureType_CanBeUsedInExpressionTrees ()
    {
      // Arrange
      var futureType = new FutureType();

      // Act
      TestDelegate action = () => Expression.New (futureType);

      // Assert
      Assert.That (action, Throws.Nothing);
    }

    private TypeBuilder CreateTypeBuilder (string typeName)
    {
      return _moduleBuilder.DefineType (typeName);
    }
  }
}