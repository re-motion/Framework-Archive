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
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class EmittableOperandProviderTest
  {
    private EmittableOperandProvider _map;

    private Type _someType;
    private FieldInfo _someFieldInfo;
    private ConstructorInfo _someConstructorInfo;
    private MethodInfo _someMethodInfo;

    [SetUp]
    public void SetUp ()
    {
      _map = new EmittableOperandProvider();

      _someType = ReflectionObjectMother.GetSomeType();
      _someFieldInfo = ReflectionObjectMother.GetSomeField ();
      _someConstructorInfo = ReflectionObjectMother.GetSomeDefaultConstructor();
      _someMethodInfo = ReflectionObjectMother.GetSomeMethod ();
    }

    [Test]
    public void AddMapping ()
    {
      CheckAddMapping (_map.AddMapping, _map.GetEmittableType, _someType);
      CheckAddMapping (_map.AddMapping, _map.GetEmittableField, _someFieldInfo);
      CheckAddMapping (_map.AddMapping, _map.GetEmittableConstructor, _someConstructorInfo);
      CheckAddMapping (_map.AddMapping, _map.GetEmittableMethod, _someMethodInfo);
    }

    [Test]
    public void AddMapping_Twice ()
    {
      CheckAddMappingTwiceThrows<Type, IEmittableOperand> (
          _map.AddMapping, _someType, "Type is already mapped.\r\nParameter name: mappedType");
      CheckAddMappingTwiceThrows<FieldInfo, IEmittableOperand> (
          _map.AddMapping, _someFieldInfo, "FieldInfo is already mapped.\r\nParameter name: mappedFieldInfo");
      CheckAddMappingTwiceThrows<ConstructorInfo, IEmittableOperand> (
          _map.AddMapping, _someConstructorInfo, "ConstructorInfo is already mapped.\r\nParameter name: mappedConstructorInfo");
      CheckAddMappingTwiceThrows<MethodInfo, IEmittableMethodOperand> (
          _map.AddMapping, _someMethodInfo, "MethodInfo is already mapped.\r\nParameter name: mappedMethodInfo");
    }
    
    [Test]
    public void GetEmittableOperand_NoMapping ()
    {
      CheckGetEmitableOperandWithNoMapping (_map.GetEmittableType, _someType, typeof (EmittableType));
      CheckGetEmitableOperandWithNoMapping (_map.GetEmittableField, _someFieldInfo, typeof (EmittableField));
      CheckGetEmitableOperandWithNoMapping (_map.GetEmittableConstructor, _someConstructorInfo, typeof (EmittableConstructor));
      CheckGetEmitableOperandWithNoMapping (_map.GetEmittableMethod, _someMethodInfo, typeof (EmittableMethod));
    }

    private void CheckAddMapping<TMappedObject, TEmittableOperand> (
        Action<TMappedObject, TEmittableOperand> addMappingMethod,
        Func<TMappedObject, TEmittableOperand> getEmittableOperandMethod,
        TMappedObject mappedObject)
        where TEmittableOperand : class, IEmittableOperand
    {
      var fakeOperand = MockRepository.GenerateStub<TEmittableOperand>();
     
      addMappingMethod (mappedObject, fakeOperand);

      var result = getEmittableOperandMethod (mappedObject);
      Assert.That (result, Is.SameAs (fakeOperand));
    }

    private void CheckAddMappingTwiceThrows<TMappedObject, TBuilder> (
        Action<TMappedObject, TBuilder> addMappingMethod, TMappedObject mappedObject, string expectedMessage)
        where TBuilder: class
    {
      addMappingMethod (mappedObject, MockRepository.GenerateStub<TBuilder>());

      Assert.That (
          () => addMappingMethod (mappedObject, MockRepository.GenerateStub<TBuilder>()),
          Throws.ArgumentException.With.Message.EqualTo (expectedMessage));
    }

    private void CheckGetEmitableOperandWithNoMapping<TMappedObject, TEmittableOperand> (
        Func<TMappedObject, TEmittableOperand> getEmittableOperandMethod, TMappedObject mappedObject, Type mappedObjectWrapper)
    {
      var result = getEmittableOperandMethod (mappedObject);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (mappedObjectWrapper));
    }
  }
}