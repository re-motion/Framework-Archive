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
using System.Reflection;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeTest
  {
    private IUnderlyingTypeStrategy _typeStrategyStub;
    private IEqualityComparer<MemberInfo> _memberInfoEqualityComparerStub;
    private IBindingFlagsEvaluator _bindingFlagsEvaluatorMock;
    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _typeStrategyStub = MockRepository.GenerateStub<IUnderlyingTypeStrategy>();
      _memberInfoEqualityComparerStub = MockRepository.GenerateStub<IEqualityComparer<MemberInfo>>();
      _bindingFlagsEvaluatorMock = MockRepository.GenerateStrictMock<IBindingFlagsEvaluator>();
      _mutableType = MutableTypeObjectMother.Create (_typeStrategyStub, _memberInfoEqualityComparerStub, _bindingFlagsEvaluatorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mutableType.AddedInterfaces, Is.Empty);
      Assert.That (_mutableType.AddedFields, Is.Empty);
      Assert.That (_mutableType.AddedConstructors, Is.Empty);
    }

    [Test]
    public void UnderlyingSystemType ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      _typeStrategyStub.Stub (stub => stub.GetUnderlyingSystemType()).Return (type);

      Assert.That (_mutableType.UnderlyingSystemType, Is.SameAs (type));
    }

    [Test]
    public void UnderlyingSystemType_ForNull ()
    {
      _typeStrategyStub.Stub (stub => stub.GetUnderlyingSystemType()).Return (null);

      Assert.That (_mutableType.UnderlyingSystemType, Is.SameAs (_mutableType));
    }

    [Test]
    public void Assembly ()
    {
      Assert.That (_mutableType.Assembly, Is.Null);
    }

    [Test]
    public void BaseType ()
    {
      var baseType = typeof (IDisposable);
      _typeStrategyStub.Stub (stub => stub.GetBaseType()).Return (baseType);

      Assert.That (_mutableType.BaseType, Is.SameAs (baseType));
    }

    [Test]
    public void IsEquivalentTo_Type_True ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      _typeStrategyStub.Stub (stub => stub.GetUnderlyingSystemType()).Return (type);

      Assert.That (_mutableType.IsEquivalentTo (type), Is.True);
    }

    [Test]
    public void IsEquivalentTo_Type_False ()
    {
      var underlyingType = ReflectionObjectMother.GetSomeType();
      var type = ReflectionObjectMother.GetSomeDifferentType();
      _typeStrategyStub.Stub (stub => stub.GetUnderlyingSystemType()).Return (underlyingType);

      Assert.That (_mutableType.IsEquivalentTo (type), Is.False);
    }

    [Test]
    public void IsEquivalentTo_MutableType_True ()
    {
      var mutableType = _mutableType;

      Assert.That (_mutableType.IsEquivalentTo (mutableType), Is.True);
    }

    [Test]
    public void IsEquivalentTo_MutableType_False ()
    {
      var mutableType = MutableTypeObjectMother.Create();

      Assert.That (_mutableType.IsEquivalentTo(mutableType), Is.False);
    }

    [Test]
    public void AddInterface ()
    {
      _typeStrategyStub.Stub (stub => stub.GetInterfaces()).Return (Type.EmptyTypes);

      _mutableType.AddInterface (typeof (IDisposable));
      _mutableType.AddInterface (typeof (IComparable));

      Assert.That (_mutableType.AddedInterfaces, Is.EqualTo (new[] { typeof (IDisposable), typeof (IComparable) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be an interface.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfNotAnInterface ()
    {
      _mutableType.AddInterface (typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Interface 'System.IDisposable' is already implemented.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfAlreadyImplemented ()
    {
      _typeStrategyStub.Stub (stub => stub.GetInterfaces()).Return (new[] { typeof (IDisposable) });

      _mutableType.AddInterface (typeof (IDisposable));
    }

    [Test]
    public void GetInterfaces ()
    {
      _typeStrategyStub.Stub (stub => stub.GetInterfaces()).Return (new[] { typeof (IDisposable) });
      _mutableType.AddInterface (typeof (IComparable));

      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (new[] { typeof (IDisposable), typeof (IComparable) }));
    }

    [Test]
    public void AddField ()
    {
      _typeStrategyStub.Stub (stub => stub.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
          .Return (new FieldInfo[0]);

      var newField = _mutableType.AddField (typeof (string), "_newField", FieldAttributes.Private);

      // Correct field info instance
      Assert.That (newField.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (newField.Name, Is.EqualTo ("_newField"));
      Assert.That (newField.FieldType, Is.EqualTo (typeof (string)));
      Assert.That (newField.Attributes, Is.EqualTo (FieldAttributes.Private));
      // Field info is stored
      Assert.That (_mutableType.AddedFields, Is.EqualTo (new[] { newField }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Field with equal name and signature already exists.\r\nParameter name: name")]
    public void AddField_ThrowsIfAlreadyExist ()
    {
      var field = MutableFieldInfoObjectMother.Create (name: "_bla", fieldType: typeof (string));
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new[] { field });
      _memberInfoEqualityComparerStub
          .Stub (stub => stub.Equals (Arg<FieldInfo>.Is.Anything, Arg<FieldInfo>.Is.Anything))
          .Return (true);

      _mutableType.AddField (typeof (string), "_bla", FieldAttributes.Private);
    }

    [Test]
    public void AddField_ReliesOnFieldSignature ()
    {
      var field = MutableFieldInfoObjectMother.Create (name: "_foo", fieldType: typeof (object));
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new[] { field });
      var attributes = FieldAttributes.Private;
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _memberInfoEqualityComparerStub.Stub (stub => stub.Equals (Arg<FieldInfo>.Is.Anything, Arg<FieldInfo>.Is.Anything))
          .Return (false);
      _bindingFlagsEvaluatorMock.Stub (stub => stub.HasRightAttributes (attributes, bindingFlags)).Return (true);

      _mutableType.AddField (typeof (string), "_foo", attributes);
      var fields = _mutableType.GetFields (bindingFlags);

      Assert.That (fields, Has.Length.EqualTo (2));
    }

    [Test]
    public void GetFields ()
    {
      var field1 = MutableFieldInfoObjectMother.Create();
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
      _typeStrategyStub.Stub (stub => stub.GetFields (bindingFlags)).Return (new[] { field1 });
      var attributes = FieldAttributes.Private;
      _bindingFlagsEvaluatorMock.Stub (stub => stub.HasRightAttributes (attributes, bindingFlags)).Return (true);

      var field2 = _mutableType.AddField (ReflectionObjectMother.GetSomeType(), "field2", attributes);
      var fields = _mutableType.GetFields (bindingFlags);

      Assert.That (fields, Is.EqualTo (new[] { field1, field2 }));
    }

    [Test]
    public void GetFields_FilterAddedWithUtility ()
    {
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new FieldInfo[0]);
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (FieldAttributes.Public, bindingFlags)).Return (false);

      _mutableType.AddField (typeof (int), "_newField", FieldAttributes.Public);
      var fields = _mutableType.GetFields (bindingFlags);

      _bindingFlagsEvaluatorMock.VerifyAllExpectations();
      Assert.That (fields, Is.Empty);
    }

    [Test]
    public void GetField ()
    {
      var field1 = MutableFieldInfoObjectMother.Create (name: "field1", fieldType: typeof (string));
      var field2 = MutableFieldInfoObjectMother.Create (name: "field2", fieldType: typeof (int));
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new[] { field1, field2 });
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<FieldAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);

      var resultField = _mutableType.GetField ("field2", BindingFlags.NonPublic | BindingFlags.Instance);

      Assert.That (resultField, Is.SameAs (field2));
    }

    [Test]
    public void GetField_NoMatch ()
    {
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new FieldInfo[0]);

      Assert.That (_mutableType.GetField ("field"), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Ambiguous field name 'foo'.")]
    public void GetField_Ambigious ()
    {
      var field1 = MutableFieldInfoObjectMother.Create (name: "foo", fieldType: typeof (string));
      var field2 = MutableFieldInfoObjectMother.Create (name: "foo", fieldType: typeof (int));
      _typeStrategyStub.Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new[] { field1, field2 });
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<FieldAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);

      _mutableType.GetField ("foo", 0);
    }

    [Test]
    public void AddConstructor ()
    {
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new ConstructorInfo[0]);
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = new[] { ParameterDeclarationObjectMother.Create(), ParameterDeclarationObjectMother.Create() };

      var ctorInfo = _mutableType.AddConstructor (attributes, parameterDeclarations);

      // Correct constructor info instance
      Assert.That (ctorInfo.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (ctorInfo.Attributes, Is.EqualTo (attributes));
      var expectedParameterInfos = new[]
                                   { new { ParameterType = parameterDeclarations[0].Type }, new { ParameterType = parameterDeclarations[1].Type } };
      var actualParameterInfos = ctorInfo.GetParameters().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      // Constructor info is stored
      Assert.That (_mutableType.AddedConstructors, Is.EqualTo (new[] { ctorInfo }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Adding static constructors are not (yet) supported.\r\nParameter name: attributes")]
    public void AddConstructor_ThrowsForStatic ()
    {
      _mutableType.AddConstructor (MethodAttributes.Static);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Constructor with equal signature already exists.\r\nParameter name: parameterDeclarations")]
    public void AddConstructor_ThrowsIfAlreadyExists ()
    {
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new ConstructorInfo[1]);
      _memberInfoEqualityComparerStub.Stub (stub => stub.Equals (null, null)).IgnoreArguments().Return (true);

      _mutableType.AddConstructor (0);
    }

    [Test]
    public void GetConstructors ()
    {
      var constructor1 = MutableConstructorInfoObjectMother.Create();
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance; // Don't return static constructors by default
      _typeStrategyStub.Stub (stub => stub.GetConstructors (bindingFlags)).Return (new[] { constructor1 });
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = new ArgumentTestHelper (7).ParameterDeclarations; // Need different signature
      _bindingFlagsEvaluatorMock.Stub (mock => mock.HasRightAttributes (attributes, bindingFlags)).Return (true);

      var constructor2 = _mutableType.AddConstructor (attributes, parameterDeclarations);
      var constructors = _mutableType.GetConstructors (bindingFlags);

      Assert.That (constructors, Is.EqualTo (new[] { constructor1, constructor2 }));
    }

    [Test]
    public void GetConstructors_FilterAddedWithUtility ()
    {
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new ConstructorInfo[0]);
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (MethodAttributes.Public, bindingFlags)).Return (false);

      _mutableType.AddConstructor (MethodAttributes.Public);
      var constructors = _mutableType.GetConstructors (bindingFlags);

      _bindingFlagsEvaluatorMock.VerifyAllExpectations();
      Assert.That (constructors, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
      "Constructor is declared by a different type: 'System.String'.\r\nParameter name: constructor")]
    public void GetMutableConstructor_NotEquivalentDeclaringType ()
    {
      var ctorStub = MockRepository.GenerateStub<ConstructorInfo>();
      ctorStub.Stub (stub => stub.DeclaringType).Return (typeof(string));

      _mutableType.GetMutableConstructor (ctorStub);
    }

    [Test]
    public void GetMutableConstructor_MutableConstructorInfo ()
    {
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new ConstructorInfo[0]);
      var ctor = _mutableType.AddConstructor (0);

      var result = _mutableType.GetMutableConstructor (ctor);

      Assert.That (result, Is.SameAs (ctor));
    }

    [Test]
    public void GetMutableConstructor_StandardConstructorInfo ()
    {
      var ctorStub = MockRepository.GenerateStub<ConstructorInfo> ();
      ctorStub.Stub (stub => stub.DeclaringType).Return (_mutableType.UnderlyingSystemType);
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new[] { ctorStub });

      var result = _mutableType.GetMutableConstructor (ctorStub);

      Assert.That (result.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (result.UnderlyingSystemConsructorInfo, Is.SameAs(ctorStub));
    }

    [Test]
    public void GetMutableConstructor_StandardConstructorInfo_Twice ()
    {
      var ctorStub = MockRepository.GenerateStub<ConstructorInfo> ();
      ctorStub.Stub (stub => stub.DeclaringType).Return (_mutableType.UnderlyingSystemType);
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new[] { ctorStub });

      var result1 = _mutableType.GetMutableConstructor (ctorStub);
      var result2 = _mutableType.GetMutableConstructor (ctorStub);

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void Accept ()
    {
      _typeStrategyStub
          .Stub (stub => stub.GetInterfaces())
          .Return (new[] { ReflectionObjectMother.GetSomeInterfaceType() });
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType ();
      _mutableType.AddInterface (addedInterface);

      _typeStrategyStub
          .Stub (stub => stub.GetFields (Arg<BindingFlags>.Is.Anything))
          .Return (new[] { ReflectionObjectMother.GetSomeField() });
      var addedFieldInfo = _mutableType.AddField (ReflectionObjectMother.GetSomeType (), "name", FieldAttributes.Private);

      _typeStrategyStub
          .Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything))
          .Return (new[] { ReflectionObjectMother.GetSomeDefaultConstructor () });
      var addedConstructorInfo = _mutableType.AddConstructor (MethodAttributes.Public, ParameterDeclarationObjectMother.Create());

      var handlerMock = MockRepository.GenerateMock<ITypeModificationHandler>();
      
      _mutableType.Accept (handlerMock);

      handlerMock.AssertWasCalled (mock => mock.HandleAddedInterface (addedInterface));
      handlerMock.AssertWasCalled (mock => mock.HandleAddedField (addedFieldInfo));
      handlerMock.AssertWasCalled (mock => mock.HandleAddedConstructor (addedConstructorInfo));
    }

    [Test]
    public void HasElementTypeImpl ()
    {
      Assert.That (_mutableType.HasElementType, Is.False);
    }

    [Test]
    public void IsByRefImpl ()
    {
      Assert.That (_mutableType.IsByRef, Is.False);
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      _typeStrategyStub.Stub (stub => stub.GetAttributeFlags()).Return (TypeAttributes.Sealed);

      Assert.That (_mutableType.Attributes, Is.EqualTo (TypeAttributes.Sealed));
    }

    [Test]
    public void GetConstructorImpl ()
    {
      var constructor1 = MutableConstructorInfoObjectMother.Create();
      var arguments = new ArgumentTestHelper (typeof (int));
      var constructor2 = MutableConstructorInfoObjectMother.CreateWithParameters (parameterDeclarations: arguments.ParameterDeclarations);
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new[] { constructor1, constructor2 });
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);

      var resultCtor = _mutableType.GetConstructor (arguments.Types);

      Assert.That (resultCtor, Is.SameAs (constructor2));
    }

    [Test]
    public void GetConstructorImpl_NoMatch ()
    {
      _typeStrategyStub.Stub (stub => stub.GetConstructors (Arg<BindingFlags>.Is.Anything)).Return (new ConstructorInfo[0]);

      Assert.That (_mutableType.GetConstructor (Type.EmptyTypes), Is.Null);
    }
  }
}