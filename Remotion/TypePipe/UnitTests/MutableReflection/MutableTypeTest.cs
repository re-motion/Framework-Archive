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
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeTest
  {
    private UnderlyingTypeDescriptor _descriptor;
    private IMemberSelector _memberSelectorMock;
    private IRelatedMethodFinder _relatedMethodFinderMock;
    private IMutableMemberFactory _mutableMemberFactoryMock;

    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _descriptor = UnderlyingTypeDescriptorObjectMother.Create (typeof (DomainType));
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();

      // Use a dynamic mock because constructor passes on _relatedMethodFinderMock to UnderlyingTypeDescriptor, which calls methods on the mock.
      // If this changes and the UnderlyingTypeDescriptor logic becomes a problem, consider injecting an ExistingMutableMemberInfoFactory instead and 
      // stubbing that.
      _relatedMethodFinderMock = MockRepository.GenerateMock<IRelatedMethodFinder>();
      _mutableMemberFactoryMock = MockRepository.GenerateStrictMock<IMutableMemberFactory>();

      _mutableType = new MutableType (_descriptor, _memberSelectorMock, _relatedMethodFinderMock, _mutableMemberFactoryMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mutableType.UnderlyingSystemType, Is.EqualTo (_descriptor.UnderlyingSystemInfo));
      Assert.That (_mutableType.DeclaringType, Is.EqualTo (_descriptor.DeclaringType));
      Assert.That (_mutableType.BaseType, Is.EqualTo (_descriptor.BaseType));
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
      Assert.That (_mutableType.Name, Is.EqualTo (_descriptor.Name));
      Assert.That (_mutableType.Namespace, Is.EqualTo (_descriptor.Namespace));
      Assert.That (_mutableType.FullName, Is.EqualTo (_descriptor.FullName));

      Assert.That (_mutableType.AddedInterfaces, Is.Empty);
      Assert.That (_mutableType.AddedFields, Is.Empty);
      Assert.That (_mutableType.AddedConstructors, Is.Empty);
      Assert.That (_mutableType.AddedMethods, Is.Empty);
    }

    [Test]
    public void Initialization_Interfaces ()
    {
      Assert.That (_descriptor.Interfaces, Is.Not.Empty);

      Assert.That (_mutableType.ExistingInterfaces, Is.EqualTo (_descriptor.Interfaces));
      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (_descriptor.Interfaces));
    }

    [Test]
    public void Initialization_Fields ()
    {
      var fields = _descriptor.Fields;
      Assert.That (fields, Is.Not.Empty); // base field, declared field
      var expectedField = fields.Single (m => m.Name == "ProtectedField");

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var mutableField = _mutableType.ExistingMutableFields.Single();

      Assert.That (mutableField.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (mutableField.UnderlyingSystemFieldInfo, Is.EqualTo (expectedField));
    }

    [Test]
    public void Initialization_Constructors ()
    {
      var ctors = _descriptor.Constructors;
      Assert.That (ctors, Has.Count.EqualTo (1));
      var expectedCtor = ctors.Single();

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var mutableCtor = _mutableType.ExistingMutableConstructors.Single();

      Assert.That (mutableCtor.UnderlyingSystemConstructorInfo, Is.EqualTo (expectedCtor));
      Assert.That (mutableCtor.DeclaringType, Is.SameAs (_mutableType));
    }

    [Test]
    public void Initialization_Methods ()
    {
      var methods = _descriptor.Methods;
      Assert.That (methods, Is.Not.Empty); // ToString(), Equals(), ...
      var expectedMethod = methods.Single (m => m.Name == "VirtualMethod");

      Assert.That (_mutableType.ExistingMutableMethods.Count, Is.EqualTo (2));
      var mutableMethod = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");

      Assert.That (mutableMethod.UnderlyingSystemMethodInfo, Is.EqualTo (expectedMethod));
      Assert.That (mutableMethod.DeclaringType, Is.SameAs (_mutableType));

      // Test that the _relatedMethodFinderMock was passed to the underlying descriptor.
      _relatedMethodFinderMock.AssertWasCalled (mock => mock.GetBaseMethod (expectedMethod));
    }

    [Test]
    public void Initialization_AbstractType ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (AbstractTypeWithOneMethod));
      Assert.That (mutableType.IsAbstract, Is.True);

      var mutableMethod = mutableType.AllMutableMethods.Single (x => x.Name == "Method");

      mutableMethod.SetBody (ctx => Expression.Default (mutableMethod.ReturnType));
      Assert.That (mutableType.IsAbstract, Is.False);
    }

    [Test]
    public void AllMutableFields ()
    {
      Assert.That (GetAllFields (_mutableType).ExistingBaseMembers, Is.Not.Empty);
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var existingField = _mutableType.ExistingMutableFields.Single();
      var addedField = AddField (_mutableType, "_addedField");

      var allFields = _mutableType.AllMutableFields.ToArray();

      Assert.That (allFields, Has.Length.EqualTo (2));
      Assert.That (allFields[0], Is.SameAs (existingField));
      Assert.That (allFields[1], Is.SameAs (addedField));
    }

    [Test]
    public void AllMutableConstructors ()
    {
      Assert.That (_descriptor.Constructors, Has.Count.EqualTo (1));
      var existingCtor = _descriptor.Constructors.Single();
      var addedCtor = AddConstructor (_mutableType, new ArgumentTestHelper (7).ParameterDeclarations); // Need different signature

      var allConstructors = _mutableType.AllMutableConstructors.ToArray();

      Assert.That (allConstructors, Has.Length.EqualTo (2));
      Assert.That (allConstructors[0].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allConstructors[0].UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (allConstructors[1], Is.SameAs (addedCtor));
    }

    [Test]
    public void AllMutableMethods ()
    {
      Assert.That (_descriptor.Methods, Is.Not.Empty);
      var existingMethods = _descriptor.Methods;
      var addedMethod = AddMethod (_mutableType, "NewMethod");

      var allMethods = _mutableType.AllMutableMethods.ToArray();

      Assert.That (allMethods, Has.Length.EqualTo (3));
      Assert.That (allMethods[0].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allMethods[0].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[0]));
      Assert.That (allMethods[1].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allMethods[1].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[1]));
      Assert.That (allMethods[2], Is.SameAs (addedMethod));
    }

    [Test]
    public new void ToString ()
    {
      // Note: ToString() is implemented in CustomType base class.
      Assert.That (_mutableType.ToString(), Is.EqualTo ("DomainType"));
    }

    [Test]
    public void ToDebugString ()
    {
      // Note: ToDebugString() is implemented in CustomType base class.
      Assert.That (_mutableType.ToDebugString(), Is.EqualTo ("MutableType = \"DomainType\""));
    }

    [Test]
    public void IsAssignableTo ()
    {
      Assert.That (_mutableType.IsAssignableTo (_mutableType), Is.True);

      var underlyingSystemType = _mutableType.UnderlyingSystemType;
      Assert.That (underlyingSystemType, Is.Not.SameAs (_mutableType));
      Assert.That (_mutableType.IsAssignableTo (underlyingSystemType), Is.True);

      Assert.That (_mutableType.BaseType, Is.SameAs (typeof (DomainTypeBase)));
      Assert.That (_mutableType.IsAssignableTo (typeof (DomainTypeBase)), Is.True);

      Assert.IsNotNull (_mutableType.BaseType); // For ReSharper...
      Assert.That (_mutableType.BaseType.BaseType, Is.SameAs (typeof (C)));
      Assert.That (_mutableType.IsAssignableTo (typeof (C)), Is.True);
      Assert.That (_mutableType.IsAssignableTo (typeof (object)), Is.True);

      Assert.That (underlyingSystemType.GetInterfaces(), Has.Member (typeof (IDomainInterface)));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDomainInterface)), Is.True);

      Assert.That (_mutableType.GetInterfaces(), Has.No.Member (typeof (IDisposable)));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDisposable)), Is.False);
      _mutableType.AddInterface (typeof (IDisposable));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDisposable)), Is.True);

      Assert.That (_mutableType.IsAssignableTo (typeof (UnrelatedType)), Is.False);
    }

    [Test]
    public void AddInterface ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();

      _mutableType.AddInterface (addedInterface);

      Assert.That (_mutableType.AddedInterfaces, Is.EqualTo (new[] { addedInterface }));
      Assert.That (_mutableType.ExistingInterfaces, Is.EqualTo (new[] { existingInterface }));
      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be an interface.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfNotAnInterface ()
    {
      _mutableType.AddInterface (typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Interface 'IDomainInterface' is already implemented.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfAlreadyImplemented ()
    {
      var existingInterface = _descriptor.Interfaces.First();
      _mutableType.AddInterface (existingInterface);
    }

    [Test]
    public void AddField ()
    {
      var name = "_newField";
      var type = ReflectionObjectMother.GetSomeType();
      var attributes = (FieldAttributes) 7;
      var fakeField = MutableFieldInfoObjectMother.Create (_mutableType);
      _mutableMemberFactoryMock.Expect (mock => mock.CreateMutableField (_mutableType, name, type, attributes)).Return (fakeField);

      var result = _mutableType.AddField (name, type, attributes);

      _mutableMemberFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeField));
      Assert.That (_mutableType.AddedFields, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void GetMutableField ()
    {
      var existingField = _descriptor.Fields.Single (m => m.Name == "ProtectedField");
      Assert.That (existingField, Is.Not.AssignableTo<MutableFieldInfo>());

      var result = _mutableType.GetMutableField (existingField);

      Assert.That (result.UnderlyingSystemFieldInfo, Is.SameAs (existingField));
      Assert.That (_mutableType.ExistingMutableFields, Has.Member (result));
    }

    [Test]
    public void AddConstructor ()
    {
      var attributes = (MethodAttributes) 7;
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = context => null;
      var constructorFake = MutableConstructorInfoObjectMother.Create (_mutableType);
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateMutableConstructor (_mutableType, attributes, parameterDeclarations, bodyProvider))
          .Return (constructorFake);

      var result = _mutableType.AddConstructor (attributes, parameterDeclarations, bodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (constructorFake));
      Assert.That (_mutableType.AddedConstructors, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void GetMutableConstructor ()
    {
      var existingCtor = _descriptor.Constructors.Single();
      Assert.That (existingCtor, Is.Not.AssignableTo<MutableConstructorInfo>());

      var result = _mutableType.GetMutableConstructor (existingCtor);

      Assert.That (result.UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (_mutableType.ExistingMutableConstructors, Has.Member (result));
    }

    [Test]
    public void AddMethod ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Public;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      Func<MethodBodyCreationContext, Expression> bodyProvider = context => null;
      var fakeMethod = MutableMethodInfoObjectMother.Create(_mutableType);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.CreateMutableMethod (
                  Arg.Is (_mutableType),
                  Arg.Is (name),
                  Arg.Is (attributes),
                  Arg.Is (returnType),
                  Arg.Is (parameterDeclarations),
                  Arg.Is (bodyProvider),
                  Arg<Action>.Is.Anything))
          .Return (fakeMethod);

      var result = _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations, bodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeMethod));
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddMethod_AbstractMethod_UpdatesIsAbstract ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType ();
      Assert.That (mutableType.IsAbstract, Is.False);

      var mutableMethod = mutableType.AddMethod (
          "AbstractMethod", MethodAttributes.Abstract, typeof (void), ParameterDeclaration.EmptyParameters, null);
      Assert.That (mutableType.IsAbstract, Is.True);

      mutableMethod.SetBody (ctx => Expression.Default (mutableMethod.ReturnType));
      Assert.That (mutableType.IsAbstract, Is.False);
    }

    [Ignore("TODO 5099")]
    [Test]
    public void AddMethod_ImplementsAbstractBaseMethod_UpdatesIsAbstract ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DerivedAbstractTypeLeavesAbstractBaseMethod));
      Assert.That (mutableType.IsAbstract, Is.True);

      mutableType.AddMethod ("Method", MethodAttributes.Virtual, typeof (void), ParameterDeclaration.EmptyParameters, ctx => Expression.Empty());
      Assert.That (mutableType.IsAbstract, Is.False);
    }

    [Test]
    public void AddAbstractMethod ()
    {
      var name = "AbstractMethod";
      var attributes = MethodAttributes.Public;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);

      var fakeMethod = MutableMethodInfoObjectMother.Create (_mutableType);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.CreateMutableMethod (
                  Arg.Is (_mutableType),
                  Arg.Is (name),
                  Arg.Is (attributes | MethodAttributes.Abstract | MethodAttributes.Virtual),
                  Arg.Is (returnType),
                  Arg.Is (parameterDeclarations),
                  Arg<Func<MethodBodyCreationContext, Expression>>.Is.Equal (null),
                  Arg<Action>.Is.Anything))
          .Return (fakeMethod);

      var result = _mutableType.AddAbstractMethod (name, attributes, returnType, parameterDeclarations);

      Assert.That (result, Is.SameAs (fakeMethod));
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void GetOrAddMutableMethod_ExistingMethod_UsesMemberCollection ()
    {
      var existingMethod = _descriptor.Methods.Single (m => m.Name == "VirtualMethod");
      Assert.That (existingMethod, Is.Not.AssignableTo<MutableMethodInfo>());

      var result = _mutableType.GetOrAddMutableMethod (existingMethod);

      Assert.That (result.UnderlyingSystemMethodInfo, Is.SameAs (existingMethod));
      Assert.That (_mutableType.ExistingMutableMethods, Has.Member (result));
    }

    [Test]
    public void GetOrAddMutableMethod_BaseMethod_CreatesNewOverride ()
    {
      var baseMethod = _descriptor.Methods.Single (m => m.Name == "ToString");
      Assert.That (baseMethod, Is.Not.AssignableTo<MutableMethodInfo>());
      var fakeOverride = MutableMethodInfoObjectMother.Create (_mutableType);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.GetOrCreateMutableMethodOverride (
                  Arg.Is (_mutableType),
                  Arg.Is (baseMethod),
                  Arg<Action>.Is.Anything,
                  out Arg<bool>.Out (true).Dummy))
          .Return (fakeOverride);

      var result = _mutableType.GetOrAddMutableMethod (baseMethod);

      Assert.That (result, Is.SameAs (fakeOverride));
      Assert.That (_mutableType.AddedMethods, Has.Member (result));
    }

    [Test]
    public void GetOrAddMutableMethod_BaseMethod_RetrievesExistingOverride ()
    {
      var baseMethod = _descriptor.Methods.Single (m => m.Name == "ToString");
      Assert.That (baseMethod, Is.Not.AssignableTo<MutableMethodInfo> ());
      var fakeOverride = MutableMethodInfoObjectMother.Create (_mutableType);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.GetOrCreateMutableMethodOverride (
                  Arg.Is (_mutableType),
                  Arg.Is (baseMethod),
                  Arg<Action>.Is.Anything,
                  out Arg<bool>.Out (false).Dummy))
          .Return (fakeOverride);

      var result = _mutableType.GetOrAddMutableMethod (baseMethod);

      Assert.That (result, Is.SameAs (fakeOverride));
      Assert.That (_mutableType.AddedMethods, Has.No.Member (result));
    }

    [Test]
    public void Accept_UnmodifiedMutableMemberHandler_WithUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var unmodfiedField = _mutableType.ExistingMutableFields.Single ();

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single ();

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var unmodfiedMethod1 = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");
      var unmodfiedMethod2 = _mutableType.ExistingMutableMethods.Single (m => m.Name == "InterfaceMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeUnmodifiedMutableMemberHandler> ();

      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodfiedField));
      handlerMock.Expect (mock => mock.HandleUnmodifiedConstructor (unmodfiedConstructor));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod1));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod2));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_UnmodifiedMutableMemberHandler_WithAddedAndModifiedExistingMembers ()
    {
      // Currently, fields cannot be modified.
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var unmodifiedField = _mutableType.ExistingMutableFields.Single();
      AddField (_mutableType, "name");

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      MutableConstructorInfoTestHelper.ModifyConstructor (_mutableType.ExistingMutableConstructors.Single ());
      AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create ());

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      MutableMethodInfoTestHelper.ModifyMethod (_mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod"));
      AddMethod (_mutableType, "AddedMethod");
      // Currently, non-virual methods cannot be modified.
      var unmodifiedMethod = _mutableType.ExistingMutableMethods.Single (m => m.Name == "InterfaceMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeUnmodifiedMutableMemberHandler> ();

      // Currently, fields and non-virtual methods cannot be modified.
      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodifiedField));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodifiedMethod));

      _mutableType.Accept (handlerMock);

      // Fields cannot currently be mutated
      //handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedField (Arg<MutableFieldInfo>.Is.Anything));
      handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedConstructor (Arg<MutableConstructorInfo>.Is.Anything));
      handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedMethod (Arg<MutableMethodInfo>.Is.NotEqual(unmodifiedMethod)));
    }

    [Test]
    public void Accept_ModificationHandler_WithAddedAndUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.GetInterfaces(), Has.Length.EqualTo (1));
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType();
      _mutableType.AddInterface (addedInterface);
      Assert.That (_mutableType.GetInterfaces(), Has.Length.EqualTo (2));

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      // There is currently no method HandleModifiedField, so we don't need to assert that the unmodified field isn't handled.
      var addedFieldInfo = AddField (_mutableType, "name");

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single();
      var addedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var unmodfiedMethod = _mutableType.ExistingMutableMethods.Single(m => m.Name == "VirtualMethod");
      var addedMethod = AddMethod (_mutableType, "AddedMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();

      handlerMock.Expect (mock => mock.HandleAddedInterface (addedInterface));

      handlerMock.Expect (mock => mock.HandleAddedField (addedFieldInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (addedConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (addedMethod));

      _mutableType.Accept (handlerMock);

      handlerMock.AssertWasNotCalled (mock => mock.HandleModifiedConstructor (unmodfiedConstructor));
      handlerMock.AssertWasNotCalled (mock => mock.HandleModifiedMethod (unmodfiedMethod));

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_ModificationHandler_WithModifiedConstructors ()
    {
      Assert.That (_mutableType.ExistingMutableConstructors, Is.Not.Empty);
      var modifiedExistingConstructorInfo = _mutableType.ExistingMutableConstructors.First();
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedExistingConstructorInfo);

      var modifiedAddedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedAddedConstructorInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();
      handlerMock.Expect (mock => mock.HandleModifiedConstructor (modifiedExistingConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (modifiedAddedConstructorInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_ModificationHandler_WithModifiedMethod ()
    {
      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var modifiedExistingMethodInfo = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedExistingMethodInfo);

      var modifiedAddedMethodInfo = AddMethod (_mutableType, "ModifiedAddedMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedAddedMethodInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();
      handlerMock.Expect (mock => mock.HandleModifiedMethod (modifiedExistingMethodInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (modifiedAddedMethodInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void GetCustomAttributeData ()
    {
      var result = _mutableType.GetCustomAttributeData ();

      Assert.That (result.Select (a => a.Type), Is.EquivalentTo (new[] { typeof (AbcAttribute) }));
      Assert.That (result, Is.SameAs (_mutableType.GetCustomAttributeData ()), "should be cached");
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAttributeFlagsImpl");

      Assert.That (result, Is.EqualTo (_descriptor.Attributes));
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
    }

    [Test]
    public void GetAllInterfaces ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single ();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType ();
      _mutableType.AddInterface (addedInterface);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllInterfaces");

      Assert.That (result, Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    public void GetAllFields ()
    {
      AddField (_mutableType, "added");
      var allFields = GetAllFields (_mutableType);
      Assert.That (allFields.AddedMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingBaseMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllFields");

      Assert.That (result, Is.SameAs (allFields));
    }

    [Test]
    public void GetAllConstructors ()
    {
      AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create ());
      var allConstructors = GetAllConstructors (_mutableType);
      Assert.That (allConstructors.AddedMembers, Is.Not.Empty);
      Assert.That (allConstructors.ExistingDeclaredMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllConstructors");

      Assert.That (result, Is.SameAs (allConstructors));
    }

    [Test]
    public void GetAllMethods ()
    {
      AddMethod (_mutableType, "Added");
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.AddedMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingBaseMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllMethods");

      Assert.That (result, Is.EqualTo (allMethods));
    }

    [Test]
    public void GetMethods_FiltersOverriddenMethods ()
    {
      var baseMethod = _descriptor.Methods.Single (m => m.Name == "ToString");
      var fakeOverride = MutableMethodInfoObjectMother.Create (
          declaringType: _mutableType,
          name: baseMethod.Name,
          methodAttributes: baseMethod.Attributes,
          parameterDeclarations: ParameterDeclaration.EmptyParameters,
          baseMethod: baseMethod);
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateMutableMethod (null, null, 0, null, null, null, null))
          .IgnoreArguments()
          .Return (fakeOverride);
      _mutableType.AddMethod ("in", 0, typeof (int), ParameterDeclaration.EmptyParameters, ctx => null);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllMethods");

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (result, Has.Member (fakeOverride));
      Assert.That (result, Has.No.Member (baseMethod));
    }

    [Test]
    public void GetMutableMember_InvalidDeclaringType ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField (() => Type.EmptyTypes);
      var ctor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new string(new char[0]));

      Assert.That (
          () => _mutableType.GetMutableField (field),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given field is declared by a different type: 'System.Type'.\r\nParameter name: field"));
      Assert.That (
          () => _mutableType.GetMutableConstructor (ctor),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given constructor is declared by a different type: 'System.String'.\r\nParameter name: constructor"));
    }

    [Test]
    public void GetMutableMember_NoMapping ()
    {
      var fieldStub = MockRepository.GenerateStub<FieldInfo> ();
      fieldStub.Stub (stub => stub.DeclaringType).Return (_mutableType);
      var ctorStub = MockRepository.GenerateStub<ConstructorInfo> ();
      ctorStub.Stub (stub => stub.DeclaringType).Return (_mutableType);

      Assert.That (
          () => _mutableType.GetMutableField (fieldStub),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("The given field cannot be modified."));
      Assert.That (
          () => _mutableType.GetMutableConstructor (ctorStub),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo ("The given constructor cannot be modified."));
    }

    private MutableFieldInfo AddField (MutableType mutableType, string name)
    {
      Assert.That (mutableType == _mutableType, "Consider adding a parameter for _mutableMemberFactoryMock");

      var fakeField = MutableFieldInfoObjectMother.Create (mutableType, name: name);
      _mutableMemberFactoryMock.Stub (stub => stub.CreateMutableField (null, "", null, 0)).IgnoreArguments().Return (fakeField);

      return mutableType.AddField ("x", typeof (int));
    }

    private MutableConstructorInfo AddConstructor (MutableType mutableType, params ParameterDeclaration[] parameterDeclarations)
    {
      Assert.That (mutableType == _mutableType, "Consider adding a parameter for _mutableMemberFactoryMock");

      var fakeCtor = MutableConstructorInfoObjectMother.CreateForNewWithParameters (mutableType, parameterDeclarations);
      _mutableMemberFactoryMock.Stub (stub => stub.CreateMutableConstructor (null, 0, null, null)).IgnoreArguments().Return (fakeCtor);

      return mutableType.AddConstructor (0, ParameterDeclaration.EmptyParameters, ctx => null);
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, string name, params ParameterDeclaration[] parameterDeclarations)
    {
      Assert.That (mutableType == _mutableType, "Consider adding a parameter for _mutableMemberFactoryMock");

      var fakeMethod = MutableMethodInfoObjectMother.Create (mutableType, name, parameterDeclarations: parameterDeclarations);
      _mutableMemberFactoryMock
          .Stub (stub => stub.CreateMutableMethod (null, "", 0, null, null, null, null))
          .IgnoreArguments()
          .Return (fakeMethod);

      return mutableType.AddMethod ("x", 0, typeof (int), ParameterDeclaration.EmptyParameters, null);
    }

    private MutableTypeMemberCollection<FieldInfo, MutableFieldInfo> GetAllFields (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<FieldInfo, MutableFieldInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_fields");
    }

    private MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> GetAllMethods (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<MethodInfo, MutableMethodInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_methods");
    }

    private MutableTypeMemberCollection<ConstructorInfo, MutableConstructorInfo> GetAllConstructors (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<ConstructorInfo, MutableConstructorInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_constructors");
    }

    public class A
    {
      // base definition
      public virtual  void OverrideHierarchy (int aaa) { }

      public virtual void FinalBaseMethodInB (int i) { }
    }

    public class B : A
    {
      // GetOrAddMutableMethod input
      public override void OverrideHierarchy (int bbb) { }

      protected internal virtual void ProtectedOrInternalVirtualNewSlotMethodInB (int parameterName) { }
      public override sealed void FinalBaseMethodInB (int i) { }
    }

    public class C : B
    {
      // base inputMethod
      public override void OverrideHierarchy (int parameterName) { }
    }

    public class DomainTypeBase : C
    {
      public int BaseField;

      public void ExistingBaseMethod () { }
    }

    [Abc]
    public class DomainType : DomainTypeBase, IDomainInterface
    {
      // ReSharper disable UnaccessedField.Global
      protected int ProtectedField;
      // ReSharper restore UnaccessedField.Global

      public DomainType ()
      {
        ProtectedField = Dev<int>.Null;
      }

      public virtual string VirtualMethod () { return ""; }

      public void InterfaceMethod () { }
    }

    public interface IDomainInterface
    {
      void InterfaceMethod ();
    }

// ReSharper disable ClassWithVirtualMembersNeverInherited.Local
    private class UnrelatedType
// ReSharper restore ClassWithVirtualMembersNeverInherited.Local
    {
      public virtual string VirtualMethod () { return ""; }
    }

    public class AbcAttribute : Attribute { }

    class ConcreteType { }
    abstract class AbstractTypeWithoutMethods { }
    abstract class AbstractTypeWithOneMethod
    {
      public abstract void Method ();
    }
    abstract class DerivedAbstractTypeLeavesAbstractBaseMethod : AbstractTypeWithOneMethod { }
    abstract class DerivedAbstractTypeOverridesAbstractBaseMethod : AbstractTypeWithOneMethod
    {
      public override void Method () { }
    }
  }
}