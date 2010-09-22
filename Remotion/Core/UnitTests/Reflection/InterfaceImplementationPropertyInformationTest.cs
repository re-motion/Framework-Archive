// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.Reflection.TestDomain;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class InterfaceImplementationPropertyInformationTest
  {
    private IPropertyInformation _implementationPropertyInformationStub;
    private IPropertyInformation _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _declarationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          _implementationPropertyInformationStub, _declarationPropertyInformationStub);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.DeclaringType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.GetOriginalDeclaringType(), Is.SameAs (typeof (bool)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttribute<string> (false)).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttribute<string> (false), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttributes<string> (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttributes<string> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.IsDefined<Attribute> (false)).Return (false);

      Assert.That (_interfaceImplementationPropertyInformation.IsDefined<Attribute> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = new PropertyInfoAdapter (typeof (string).GetProperty ("Length"));
      _implementationPropertyInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (bool))).Return (propertyInfoAdapter);

      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceImplementation (typeof (bool)), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceDeclaration(), Is.SameAs (_declarationPropertyInformationStub));
    }

    [Test]
    public void GetIndexParameters ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetIndexParameters()).Return (new ParameterInfo[0]);

      Assert.That (_interfaceImplementationPropertyInformation.GetIndexParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CanBeSetFromOutside ()
    {
      _declarationPropertyInformationStub.Stub (stub => stub.CanBeSetFromOutside).Return (false);

      Assert.That (_interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.SetValue (instance, value, null)).WhenCalled (
          mi => instance.ImplicitInterfaceScalar = value);

      _interfaceImplementationPropertyInformation.SetValue (instance, value, null);
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.GetValue (instance, null)).Return (value);

      Assert.That (_interfaceImplementationPropertyInformation.GetValue (instance, null), Is.SameAs (value));
    }


    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("ImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "get_ImplicitProperty", getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "set_ImplicitProperty", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.ExplicitProperty", 
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), 
          "Remotion.Reflection.TestDomain.IInterfaceToImplement.get_ExplicitProperty", getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), 
          "Remotion.Reflection.TestDomain.IInterfaceToImplement.set_ExplicitProperty", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ReadOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "get_ReadOnlyImplicitProperty", getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("WriteOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "set_WriteOnlyImplicitProperty", getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ReadOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface),
          "Remotion.Reflection.TestDomain.IInterfaceToImplement.get_ReadOnlyExplicitProperty", getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
      
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("WriteOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface),
          "Remotion.Reflection.TestDomain.IInterfaceToImplement.set_WriteOnlyExplicitProperty", getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingGetAccessor ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingGetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (MethodInfoAdapter), typeof (ClassImplementingInterface), "get_PropertyAddingGetAccessor", getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "set_PropertyAddingGetAccessor", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitReadOnlyOnlyPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "get_PropertyAddingSetAccessor", getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (MethodInfoAdapter), typeof (ClassImplementingInterface), "set_PropertyAddingSetAccessor", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (true);
      CheckMethodInformation (
          typeof (MethodInfoAdapter), typeof (ClassImplementingInterface), "get_PropertyAddingPrivateGetAccessor", getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "set_PropertyAddingPrivateGetAccessor", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

     Assert.That(interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "set_PropertyAddingPrivateGetAccessor", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "get_PropertyAddingPrivateSetAccessor", getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (true);
      CheckMethodInformation (
          typeof (MethodInfoAdapter), typeof (ClassImplementingInterface), "set_PropertyAddingPrivateSetAccessor", setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation), typeof (ClassImplementingInterface), "get_PropertyAddingPrivateSetAccessor", getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("ImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.ExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ReadOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("WriteOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("ReadOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          new PropertyInfoAdapter (
              typeof (ClassImplementingInterface).GetProperty ("Remotion.Reflection.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("WriteOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingPrivateSetAccessor ()
    {
      var implementationPropertyInfo = new PropertyInfoAdapter (typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = new PropertyInfoAdapter (typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void To_String ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.ToString(), Is.EqualTo ("Test(impl of 'Boolean')"));
    }

    private void CheckMethodInformation (
        Type expectedType, Type expectedPropertyDeclaringType, string expectedPropertyName, IMethodInformation actualMethodInformation)
    {
      Assert.That (actualMethodInformation, Is.TypeOf (expectedType));
      Assert.That (actualMethodInformation.DeclaringType, Is.SameAs (expectedPropertyDeclaringType));
      Assert.That (actualMethodInformation.Name, Is.EqualTo (expectedPropertyName));
    }
  }
}