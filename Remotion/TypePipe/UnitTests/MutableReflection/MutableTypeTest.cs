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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeTest
  {
    private UnderlyingTypeDescriptor _descriptor;
    private IMemberSelector _memberSelectorMock;

    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _descriptor = UnderlyingTypeDescriptorObjectMother.Create(originalType: typeof (DomainType));
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector> ();

      _mutableType = new MutableType (_descriptor, _memberSelectorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mutableType.AddedInterfaces, Is.Empty);
      Assert.That (_mutableType.AddedFields, Is.Empty);
      Assert.That (_mutableType.AddedConstructors, Is.Empty);
      Assert.That (_mutableType.AddedMethods, Is.Empty);
    }

    [Test]
    public void Initialization_Interfaces ()
    {
      Assert.That (_descriptor.Interfaces, Is.Not.Empty);

      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (_descriptor.Interfaces));
    }

    [Test]
    public void Initialization_Fields ()
    {
      var fields = _descriptor.Fields;
      Assert.That (fields, Is.Not.Empty); // base field, declared field
      var expectedField = fields.Single (m => m.Name == "ProtectedField");

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var mutableField = _mutableType.ExistingMutableFields.Single ();

      Assert.That (mutableField.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (mutableField.UnderlyingSystemFieldInfo, Is.EqualTo (expectedField));
    }

    [Test]
    public void Initialization_Constructors ()
    {
      var ctors = _descriptor.Constructors;
      Assert.That (ctors, Has.Count.EqualTo (1));
      var expectedCtor = ctors.Single ();

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
      var expectedMethod = methods.Single (m => m.Name == "PublicMethod");

      Assert.That (_mutableType.ExistingMutableMethods.Count, Is.EqualTo(1));
      var mutableMethod = _mutableType.ExistingMutableMethods.Single ();

      Assert.That (mutableMethod.UnderlyingSystemMethodInfo, Is.EqualTo (expectedMethod));
      Assert.That (mutableMethod.DeclaringType, Is.SameAs (_mutableType));
    }

    [Test]
    public void AllMutableFields ()
    {
      Assert.That (GetAllFields (_mutableType).ExistingBaseMembers, Is.Not.Empty);
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var existingField = _mutableType.ExistingMutableFields.Single ();
      var addedField = _mutableType.AddField (ReflectionObjectMother.GetSomeType(), "_addedField");

      var allFields = _mutableType.AllMutableFields.ToArray();

      Assert.That (allFields, Has.Length.EqualTo (2));
      Assert.That (allFields[0], Is.SameAs (existingField));
      Assert.That (allFields[1], Is.SameAs (addedField));
    }

    [Test]
    public void AllMutableConstructors ()
    {
      Assert.That (_descriptor.Constructors, Has.Count.EqualTo (1));
      var existingCtor = _descriptor.Constructors.Single ();
      var addedCtor = AddConstructor (_mutableType, new ArgumentTestHelper (7).ParameterDeclarations); // Need different signature

      var allConstructors = _mutableType.AllMutableConstructors.ToArray();

      Assert.That (allConstructors, Has.Length.EqualTo (2));
      Assert.That (allConstructors[0].DeclaringType, Is.SameAs(_mutableType));
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

      Assert.That (allMethods, Has.Length.EqualTo (2));
      Assert.That (allMethods[0].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allMethods[0].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[0]));
      Assert.That (allMethods[1], Is.SameAs (addedMethod));
    }

    [Test]
    public void UnderlyingSystemType ()
    {
      Assert.That (_descriptor.UnderlyingSystemType, Is.Not.Null);

      Assert.That (_mutableType.UnderlyingSystemType, Is.SameAs (_descriptor.UnderlyingSystemType));
    }

    [Test]
    public void IsNewType ()
    {
      Assert.That (_mutableType.IsNewType, Is.False);
    }

    [Test]
    public void Assembly ()
    {
      Assert.That (_mutableType.Assembly, Is.Null);
    }

    [Test]
    public void Module ()
    {
      Assert.That (_mutableType.Module, Is.Null);
    }

    [Test]
    public void BaseType ()
    {
      Assert.That (_descriptor.BaseType, Is.Not.Null);

      Assert.That (_mutableType.BaseType, Is.SameAs (_descriptor.BaseType));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_descriptor.Name, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.Name, Is.EqualTo (_descriptor.Name));
    }

    [Test]
    public void Namespace ()
    {
      Assert.That (_descriptor.Namespace, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.Namespace, Is.EqualTo (_descriptor.Namespace));
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_descriptor.FullName, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.FullName, Is.EqualTo (_descriptor.FullName));
    }

    [Test]
    public new void ToString ()
    {
      Assert.That (_descriptor.StringRepresentation, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.ToString(), Is.EqualTo (_descriptor.StringRepresentation));
    }

    [Test]
    public void ToDebugString ()
    {
      Assert.That (_descriptor.StringRepresentation, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.ToDebugString(), Is.EqualTo ("MutableType = \"" + _descriptor.Name + "\""));
    }

    [Test]
    public void IsEquivalentTo_Type_False ()
    {
      var type = ReflectionObjectMother.GetSomeDifferentType();

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
      var interface1 = ReflectionObjectMother.GetSomeInterfaceType();
      var interface2 = ReflectionObjectMother.GetSomeDifferentInterfaceType();

      _mutableType.AddInterface (interface1);
      _mutableType.AddInterface (interface2);

      Assert.That (_mutableType.AddedInterfaces, Is.EqualTo (new[] { interface1, interface2 }));
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
    public void GetInterfaces ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single ();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType ();
      _mutableType.AddInterface (addedInterface);

      var interfaces = _mutableType.GetInterfaces();

      Assert.That (interfaces, Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    public void GetInterface_NoMatch ()
    {
      Assert.That (_mutableType.GetInterfaces().Count (), Is.EqualTo (1));

      var result = _mutableType.GetInterface ("IMyInterface", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive_NoMatch ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.GetInterfaces().Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("Imyinterface", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.GetInterfaces().Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("IMyInterface", false);

      Assert.That (result, Is.SameAs (typeof (IMyInterface)));
    }

    [Test]
    public void GetInterface_IgnoreCase ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.GetInterfaces ().Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("Imyinterface", true);

      Assert.That (result, Is.SameAs (typeof (IMyInterface)));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Ambiguous interface name 'Imyinterface'.")]
    public void GetInterface_IgnoreCase_Ambiguous ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      _mutableType.AddInterface (typeof (Imyinterface));
      Assert.That (_mutableType.GetInterfaces ().Count (), Is.EqualTo (3));

      _mutableType.GetInterface ("Imyinterface", true);
    }

    [Test]
    public void AddField ()
    {
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
      var field = _mutableType.ExistingMutableFields.Single ();
      _mutableType.AddField (field.FieldType, field.Name, FieldAttributes.Private);
    }

    [Test]
    public void AddField_ReliesOnFieldSignature ()
    {
      var field = _mutableType.ExistingMutableFields.Single();
      Assert.That (field.FieldType, Is.Not.SameAs (typeof (string)));

      _mutableType.AddField (typeof (string), field.Name, FieldAttributes.Private);

      Assert.That (_mutableType.AddedFields, Has.Count.EqualTo (1));
    }

    [Test]
    public void GetFields ()
    {
      _mutableType.AddField (typeof (int), "added");
      var allFields = GetAllFields (_mutableType);
      Assert.That (allFields.AddedMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingBaseMembers, Is.Not.Empty);
      
      var bindingAttr = BindingFlags.NonPublic;
      var fakeResult = new[] { ReflectionObjectMother.GetSomeField() };
      _memberSelectorMock
          .Expect (mock => mock.SelectFields (allFields, bindingAttr))
          .Return (fakeResult);

      var result = _mutableType.GetFields (bindingAttr);

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void GetField_Added ()
    {
      var addedField = _mutableType.AddField (typeof (int), "added");

      var bindingAttr = BindingFlags.NonPublic;
      var fakeResult = ReflectionObjectMother.GetSomeField ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleField (Arg<IEnumerable<FieldInfo>>.List.Equal (new[] { addedField }), Arg.Is (bindingAttr)))
          .Return (fakeResult);

      var resultField = _mutableType.GetField (addedField.Name, bindingAttr);

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (resultField, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetField_ExistingDeclared ()
    {
      var allFields = GetAllFields (_mutableType);
      Assert.That (allFields.ExistingDeclaredMembers, Is.Not.Empty);
      var existingDeclaredField = allFields.ExistingDeclaredMembers.Single ();

      var bindingAttr = BindingFlags.NonPublic;

      var fakeResult = ReflectionObjectMother.GetSomeField ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleField (Arg<IEnumerable<FieldInfo>>.List.Equal (new[] { existingDeclaredField }), Arg.Is (bindingAttr)))
          .Return (fakeResult);

      var resultField = _mutableType.GetField (existingDeclaredField.Name, bindingAttr);

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (resultField, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetField_ExistingBase ()
    {
      var allFields = GetAllFields (_mutableType);
      Assert.That (allFields.ExistingBaseMembers, Is.Not.Empty);
      var existingBaseField = allFields.ExistingBaseMembers.Single ();

      var bindingAttr = BindingFlags.NonPublic;
      var fakeResult = ReflectionObjectMother.GetSomeField ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleField (Arg<IEnumerable<FieldInfo>>.List.Equal (new[] { existingBaseField }), Arg.Is (bindingAttr)))
          .Return (fakeResult);

      var resultField = _mutableType.GetField (existingBaseField.Name, bindingAttr);
      Assert.That (resultField, Is.SameAs (fakeResult));
    }

    [Test]
    public void AddConstructor ()
    {
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.Parameters, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
        Assert.That (context.This.Type, Is.SameAs (_mutableType));

        return fakeBody;
      };

      var ctorInfo = _mutableType.AddConstructor (attributes, parameterDeclarations.AsOneTime(), bodyProvider);

      // Correct constructor info instance
      Assert.That (ctorInfo.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (ctorInfo.Attributes, Is.EqualTo (attributes));
      var expectedParameterInfos =
          new[]
          {
              new { ParameterType = parameterDeclarations[0].Type },
              new { ParameterType = parameterDeclarations[1].Type }
          };
      var actualParameterInfos = ctorInfo.GetParameters().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Block (typeof (void), fakeBody);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, ctorInfo.Body);

      // Constructor info is stored
      Assert.That (_mutableType.AddedConstructors, Is.EqualTo (new[] { ctorInfo }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Adding static constructors is not (yet) supported.\r\nParameter name: attributes")]
    public void AddConstructor_ThrowsForStatic ()
    {
      _mutableType.AddConstructor (MethodAttributes.Static, ParameterDeclaration.EmptyParameters, context => { throw new NotImplementedException(); });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Constructor with equal signature already exists.\r\nParameter name: parameterDeclarations")]
    public void AddConstructor_ThrowsIfAlreadyExists ()
    {
      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      Assert.That (_mutableType.ExistingMutableConstructors.Single ().GetParameters (), Is.Empty);

      _mutableType.AddConstructor (0, ParameterDeclaration.EmptyParameters, context => Expression.Empty());
    }

    [Test]
    public void GetConstructors ()
    {
      AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());
      var allConstructors = GetAllConstructors (_mutableType);
      Assert.That (allConstructors.AddedMembers, Is.Not.Empty);
      Assert.That (allConstructors.ExistingDeclaredMembers, Is.Not.Empty);

      var bindingAttr = BindingFlags.NonPublic;
      var fakeResult = new[] { ReflectionObjectMother.GetSomeConstructor () };
      _memberSelectorMock
          .Expect (mock => mock.SelectMethods (allConstructors, bindingAttr))
          .Return (fakeResult);

      var result = _mutableType.GetConstructors (bindingAttr);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void GetMutableConstructor ()
    {
      var existingCtor = _descriptor.Constructors.Single ();
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
      var returnType = typeof (object);
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (int));
      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.Parameters, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
        Assert.That (context.IsStatic, Is.False);
        Assert.That (context.This.Type, Is.SameAs (_mutableType));

        return fakeBody;
      };

      var method = _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations.AsOneTime(), bodyProvider);

      // Correct method info instance
      Assert.That (method.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (method.UnderlyingSystemMethodInfo, Is.SameAs (method));
      Assert.That (method.Name, Is.EqualTo (name));
      Assert.That (method.ReturnType, Is.EqualTo (returnType));
      Assert.That (method.Attributes, Is.EqualTo (attributes));
      Assert.That (method.IsGenericMethod, Is.False);
      Assert.That (method.IsGenericMethodDefinition, Is.False);
      Assert.That (method.ContainsGenericParameters, Is.False);
      var expectedParameterInfos =
          new[]
          {
              new { ParameterType = parameterDeclarations[0].Type },
              new { ParameterType = parameterDeclarations[1].Type }
          };
      var actualParameterInfos = method.GetParameters ().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Convert (fakeBody, returnType);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);

      // Method info is stored
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { method }));
    }

    [Test]
    public void AddMethod_Static ()
    {
      var name = "StaticMethod";
      var attributes = MethodAttributes.Static;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (returnType);
      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.IsStatic, Is.True);
        return fakeBody;
      };

      _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations, bodyProvider);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method 'PublicMethod' with equal signature already exists.\r\nParameter name: name")]
    public void AddMethod_ThrowsIfAlreadyExists ()
    {
      var method = _mutableType.ExistingMutableMethods.Single (m => m.Name == "PublicMethod");
      Assert.That (method, Is.Not.Null);
      Assert.That (method.ReturnType, Is.SameAs(typeof(void)));
      Assert.That (method.GetParameters(), Is.Empty);

      _mutableType.AddMethod ("PublicMethod", 0, typeof (void), ParameterDeclaration.EmptyParameters, cx => Expression.Empty());
    }

    [Test]
    public void AddMethod_AllowsShadowing ()
    {
      var baseMethod = GetBaseMethod (_mutableType, "ToString");
      Assert.That (baseMethod, Is.Not.Null);
      Assert.That (baseMethod.DeclaringType, Is.SameAs (typeof (object)));

      var newMethod = _mutableType.AddMethod ("ToString", 0, typeof (string), ParameterDeclaration.EmptyParameters, ctx => Expression.Constant ("string"));

      Assert.That (newMethod, Is.Not.Null.And.Not.EqualTo (baseMethod));
      Assert.That (newMethod.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (_mutableType.AddedMethods, Has.Member (newMethod));
    }

    [Test]
    public void GetMethods ()
    {
      AddMethod (_mutableType, "Added");
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.AddedMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingBaseMembers, Is.Not.Empty);

      var bindingAttr = BindingFlags.NonPublic;
      var fakeResult = new[] { ReflectionObjectMother.GetSomeMethod () };
      _memberSelectorMock
          .Expect (mock => mock.SelectMethods (allMethods, bindingAttr))
          .Return (fakeResult);

      var result = _mutableType.GetMethods (bindingAttr);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void GetMutableMethod ()
    {
      var existingMethod = _descriptor.Methods.Single (m => m.Name == "PublicMethod");
      Assert.That (existingMethod, Is.Not.AssignableTo<MutableMethodInfo> ());

      var result = _mutableType.GetMutableMethod (existingMethod);

      Assert.That (result.UnderlyingSystemMethodInfo, Is.SameAs (existingMethod));
      Assert.That (_mutableType.ExistingMutableMethods, Has.Member (result));
    }

    [Test]
    public void Accept_WithAddedAndUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.GetInterfaces(), Has.Length.EqualTo (1));
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType ();
      _mutableType.AddInterface (addedInterface);
      Assert.That (_mutableType.GetInterfaces (), Has.Length.EqualTo (2));

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo(1));
      var unmodfiedField = _mutableType.ExistingMutableFields.Single();
      var addedFieldInfo = _mutableType.AddField (ReflectionObjectMother.GetSomeType (), "name", FieldAttributes.Private);

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single();
      var addedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (1));
      var unmodfiedMethod = _mutableType.ExistingMutableMethods.Single();
      var addedMethod = AddMethod (_mutableType, "AddedMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeMemberHandler>();

      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodfiedField));
      handlerMock.Expect (mock => mock.HandleUnmodifiedConstructor (unmodfiedConstructor));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod));

      handlerMock.Expect (mock => mock.HandleAddedField (addedFieldInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (addedConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (addedMethod));
      
      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_WithModifiedConstructors ()
    {
      Assert.That (_mutableType.ExistingMutableConstructors, Is.Not.Empty);
      var modifiedExistingConstructorInfo = _mutableType.ExistingMutableConstructors.First();
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedExistingConstructorInfo);

      var modifiedAddedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedAddedConstructorInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeMemberHandler> ();
      handlerMock.Expect (mock => mock.HandleModifiedConstructor (modifiedExistingConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (modifiedAddedConstructorInfo));
      SetupUnmodifiedMembersExpectations (handlerMock);

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_WitModifiedMethod ()
    {
      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (1));
      var modifiedExistingMethodInfo = _mutableType.ExistingMutableMethods.Single ();
      MutableMethodInfoTestHelper.ModifyMethod (modifiedExistingMethodInfo);

      var modifiedAddedMethodInfo = AddMethod (_mutableType, "ModifiedAddedMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedAddedMethodInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeMemberHandler> ();
      handlerMock.Expect (mock => mock.HandleModifiedMethod (modifiedExistingMethodInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (modifiedAddedMethodInfo));
      SetupUnmodifiedMembersExpectations (handlerMock);

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetElementType ()
    {
      Assert.That (_mutableType.GetElementType(), Is.Null);
    }

    [Test]
    public void HasElementTypeImpl ()
    {
      Assert.That (_mutableType.HasElementType, Is.False);
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
    }

    [Test]
    public void IsByRefImpl ()
    {
      Assert.That (_mutableType.IsByRef, Is.False);
    }

    [Test]
    public void IsArrayImpl ()
    {
      Assert.That (_mutableType.IsArray, Is.False);
    }

    [Test]
    public void IsPointerImpl ()
    {
      Assert.That (_mutableType.IsPointer, Is.False);
    }

    [Test]
    public void IsPrimitiveImpl ()
    {
      Assert.That (_mutableType.IsPrimitive, Is.False);
    }

    [Test]
    public void IsCOMObjectImpl ()
    {
      Assert.That (_mutableType.IsCOMObject, Is.False);
    }

    [Test]
    public void GetConstructorImpl ()
    {
      var addedConstructor = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());
      var allConstructors = GetAllConstructors (_mutableType);
      Assert.That (allConstructors.ExistingDeclaredMembers, Is.Not.Empty);
      var existingDeclaredConstructor = allConstructors.ExistingDeclaredMembers.Single ();

      var binder = MockRepository.GenerateStub<Binder> ();
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeConstructor ();
      _memberSelectorMock
          .Expect (
              mock => mock.SelectSingleMethod (
                  Arg.Is (binder),
                  Arg.Is (bindingAttr), 
                  Arg<IEnumerable<ConstructorInfo>>.List.Equal (new ConstructorInfo[] { existingDeclaredConstructor, addedConstructor }),
                  Arg.Is (typesOrNull),
                  Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultConstructor = CallGetConstructorImpl (_mutableType, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultConstructor, Is.SameAs (fakeResult));
    }
    
    [Test]
    public void GetConstructorImpl_NullBinder ()
    {
      var addedConstructor = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create ());
      var allConstructors = GetAllConstructors (_mutableType);
      Assert.That (allConstructors.ExistingDeclaredMembers, Is.Not.Empty);
      var existingDeclaredConstructor = allConstructors.ExistingDeclaredMembers.Single ();

      Binder binder = null;
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeConstructor ();
      _memberSelectorMock
          .Expect (
              mock => mock.SelectSingleMethod (
                  Arg.Is (Type.DefaultBinder),
                  Arg.Is (bindingAttr),
                  Arg<IEnumerable<ConstructorInfo>>.List.Equal (new ConstructorInfo[] { existingDeclaredConstructor, addedConstructor }),
                  Arg.Is (typesOrNull),
                  Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultConstructor = CallGetConstructorImpl (_mutableType, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultConstructor, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetMethodImpl_Added ()
    {
      var addedMethod = AddMethod (_mutableType, "Added");

      var binder = MockRepository.GenerateStub<Binder> ();
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeMethod ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleMethod (
              Arg.Is (binder),
              Arg.Is (bindingAttr), 
              Arg<IEnumerable<MethodInfo>>.List.Equal (new MethodInfo[] { addedMethod }),
              Arg.Is (typesOrNull), 
              Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultMethod = CallGetMethodImpl (_mutableType, addedMethod.Name, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultMethod, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetMethodImpl_ExistingDeclared ()
    {
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.ExistingDeclaredMembers, Is.Not.Empty);
      var existingDeclaredMethod = allMethods.ExistingDeclaredMembers.Single ();

      var binder = MockRepository.GenerateStub<Binder> ();
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeMethod ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleMethod (
              Arg.Is (binder),
              Arg.Is (bindingAttr),
              Arg<IEnumerable<MethodInfo>>.List.Equal (new MethodInfo[] { existingDeclaredMethod }),
              Arg.Is (typesOrNull),
              Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultMethod = CallGetMethodImpl (_mutableType, existingDeclaredMethod.Name, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultMethod, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetMethodImpl_ExistingBase ()
    {
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.ExistingBaseMembers, Is.Not.Empty);
      var existingBaseMethod = allMethods.ExistingBaseMembers.First ();

      var binder = MockRepository.GenerateStub<Binder> ();
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeMethod ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleMethod (
              Arg.Is (binder),
              Arg.Is (bindingAttr),
              Arg<IEnumerable<MethodInfo>>.List.Equal (new[] { existingBaseMethod }),
              Arg.Is (typesOrNull),
              Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultMethod = CallGetMethodImpl (_mutableType, existingBaseMethod.Name, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultMethod, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetMethodImpl_NullBinder ()
    {
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.ExistingBaseMembers, Is.Not.Empty);
      var existingBaseMethod = allMethods.ExistingBaseMembers.First ();

      Binder binder = null;
      var callingConvention = CallingConventions.Any;
      var bindingAttr = BindingFlags.NonPublic;
      var typesOrNull = new[] { ReflectionObjectMother.GetSomeType () };
      var modifiersOrNull = new[] { new ParameterModifier (1) };

      var fakeResult = ReflectionObjectMother.GetSomeMethod ();
      _memberSelectorMock
          .Expect (mock => mock.SelectSingleMethod (
              Arg.Is (Type.DefaultBinder),
              Arg.Is (bindingAttr),
              Arg<IEnumerable<MethodInfo>>.List.Equal (new[] { existingBaseMethod }),
              Arg.Is (typesOrNull),
              Arg.Is (modifiersOrNull)))
          .Return (fakeResult);

      var resultMethod = CallGetMethodImpl (_mutableType, existingBaseMethod.Name, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull);

      _memberSelectorMock.VerifyAllExpectations ();
      Assert.That (resultMethod, Is.SameAs (fakeResult));
    }

    [Test]
    public void VirtualMethodsImplementedByType ()
    {
      // None of these members should throw an exception 
      Dev.Null = _mutableType.MemberType;
      Dev.Null = _mutableType.DeclaringType;
      Dev.Null = _mutableType.DeclaringMethod;
      Dev.Null = _mutableType.ReflectedType;
      Dev.Null = _mutableType.IsGenericType;
      Dev.Null = _mutableType.IsGenericTypeDefinition;
      Dev.Null = _mutableType.IsGenericParameter;
      Dev.Null = _mutableType.ContainsGenericParameters;

      Dev.Null = _mutableType.IsValueType; // IsValueTypeImpl()
      Dev.Null = _mutableType.IsContextful; // IsContextfulImpl()
      Dev.Null = _mutableType.IsMarshalByRef; // IsMarshalByRefImpl()

      _mutableType.FindInterfaces ((type, filterCriteria) => true, filterCriteria: null);
      _mutableType.GetEvents ();
      _mutableType.GetMember ("name", BindingFlags.Default);
      _mutableType.GetMember ("name", MemberTypes.All, BindingFlags.Default);
      _mutableType.IsSubclassOf (null);
      _mutableType.IsInstanceOfType (null);
      _mutableType.IsAssignableFrom (null);

      _memberSelectorMock
          .Stub (stub => stub.SelectMethods (Arg<IEnumerable<MethodInfo>>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (new MethodInfo[0]);
      _memberSelectorMock
          .Stub (stub => stub.SelectMethods (Arg<IEnumerable<ConstructorInfo>>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (new ConstructorInfo[0]);
      _memberSelectorMock
          .Stub (stub => stub.SelectFields (Arg<IEnumerable<FieldInfo>>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (new FieldInfo[0]);
      _mutableType.FindMembers (MemberTypes.All, BindingFlags.Default, filter: null, filterCriteria: null);
    }

    [Test]
    public void UnsupportedMembers ()
    {
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.MetadataToken, "Property", "MetadataToken");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GUID, "Property", "GUID");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.AssemblyQualifiedName, "Property", "AssemblyQualifiedName");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.StructLayoutAttribute, "Property", "StructLayoutAttribute");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GenericParameterAttributes, "Property", "GenericParameterAttributes");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GenericParameterPosition, "Property", "GenericParameterPosition");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.TypeHandle, "Property", "TypeHandle");

      CheckThrowsNotSupported (() => _mutableType.GetDefaultMembers (), "Method", "GetDefaultMembers");
      CheckThrowsNotSupported (() => _mutableType.GetInterfaceMap (null), "Method", "GetInterfaceMap");
      CheckThrowsNotSupported (() => _mutableType.InvokeMember (null, 0, null, null, null), "Method", "InvokeMember");
      CheckThrowsNotSupported (() => _mutableType.MakePointerType(), "Method", "MakePointerType");
      CheckThrowsNotSupported (() => _mutableType.MakeByRefType(), "Method", "MakeByRefType");
      CheckThrowsNotSupported (() => _mutableType.MakeArrayType(), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _mutableType.MakeArrayType (7), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _mutableType.GetArrayRank(), "Method", "GetArrayRank");
      CheckThrowsNotSupported (() => _mutableType.GetGenericParameterConstraints(), "Method", "GetGenericParameterConstraints");
      CheckThrowsNotSupported (() => _mutableType.MakeGenericType(), "Method", "MakeGenericType");
      CheckThrowsNotSupported (() => _mutableType.GetGenericArguments(), "Method", "GetGenericArguments");
      CheckThrowsNotSupported (() => _mutableType.GetGenericTypeDefinition (), "Method", "GetGenericTypeDefinition");
    }

    private void CheckThrowsNotSupported(TestDelegate memberInvocation, string memberType, string memberName)
    {
      var message = string.Format ("{0} MutableType.{1} is not supported.", memberType, memberName);
      Assert.That (memberInvocation, Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
    }

    private MutableConstructorInfo AddConstructor (MutableType mutableType, params ParameterDeclaration[] parameterDeclarations)
    {
      return mutableType.AddConstructor (MethodAttributes.Public, parameterDeclarations.AsOneTime(), context => Expression.Empty());
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, string name, params ParameterDeclaration[] parameterDeclarations)
    {
      var returnType = ReflectionObjectMother.GetSomeType();
      var body = ExpressionTreeObjectMother.GetSomeExpression (returnType);

      return mutableType.AddMethod (name, MethodAttributes.Public, returnType, parameterDeclarations.AsOneTime(), ctx => body);
    }

    private MethodInfo CallGetMethodImpl (
        MutableType mutableType,
        string name,
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callingConvention,
        Type[] typesOrNull,
        ParameterModifier[] modifiersOrNull)
    {
      var arguments = new object[] { name, bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull };
      return (MethodInfo) PrivateInvoke.InvokeNonPublicMethod (mutableType, "GetMethodImpl", arguments);
    }

    private ConstructorInfo CallGetConstructorImpl (
        MutableType mutableType,
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callingConvention,
        Type[] typesOrNull,
        ParameterModifier[] modifiersOrNull)
    {
      var arguments = new object[] { bindingAttr, binder, callingConvention, typesOrNull, modifiersOrNull };
      return (ConstructorInfo) PrivateInvoke.InvokeNonPublicMethod (mutableType, "GetConstructorImpl", arguments);
    }

    private void SetupUnmodifiedMembersExpectations (IMutableTypeMemberHandler handlerMock)
    {
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var unmodfiedField = _mutableType.ExistingMutableFields.Single ();

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single ();

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (1));
      var unmodfiedMethod = _mutableType.ExistingMutableMethods.Single ();

      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodfiedField)).Repeat.Any ();
      handlerMock.Expect (mock => mock.HandleUnmodifiedConstructor (unmodfiedConstructor)).Repeat.Any ();
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod)).Repeat.Any ();
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

    private MethodInfo GetBaseMethod (MutableType mutableType, string name)
    {
      return GetAllMethods (mutableType).ExistingBaseMembers.Single (mi => mi.Name == name);
    }

    public class DomainTypeBase
    {
      public int BaseField;

      public void ExistingBaseMethod ()
      {
      }
    }

    public class DomainType : DomainTypeBase, IDomainInterface
    {
// ReSharper disable UnaccessedField.Global
      protected int ProtectedField;
// ReSharper restore UnaccessedField.Global

      public DomainType ()
      {
        ProtectedField = Dev<int>.Null;
      }

      public virtual void PublicMethod () { }
    }

    public interface IDomainInterface
    {
    }

    interface IMyInterface { }
    interface Imyinterface { }
  }
}