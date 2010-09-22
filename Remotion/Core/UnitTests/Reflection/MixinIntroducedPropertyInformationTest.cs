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
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class MixinIntroducedPropertyInformationTest
  {
    private IPropertyInformation _implementationPropertyInformationStub;
    private MixinIntroducedPropertyInformation _mixinIntroducedPropertyInformation;
    private IPropertyInformation _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _declarationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          _implementationPropertyInformationStub, _declarationPropertyInformationStub);
      _mixinIntroducedPropertyInformation = new MixinIntroducedPropertyInformation (_interfaceImplementationPropertyInformation);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_mixinIntroducedPropertyInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (object));

      Assert.That (_mixinIntroducedPropertyInformation.DeclaringType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (object));

      Assert.That (_mixinIntroducedPropertyInformation.GetOriginalDeclaringType(), Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var objToReturn = new object();
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttribute<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedPropertyInformation.GetCustomAttribute<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new object[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttributes<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedPropertyInformation.GetCustomAttributes<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.IsDefined<object> (false)).Return (false);

      Assert.That (_mixinIntroducedPropertyInformation.IsDefined<object> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = new PropertyInfoAdapter (typeof (string).GetProperty ("Length"));
      _implementationPropertyInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (object))).Return (propertyInfoAdapter);

      Assert.That (_mixinIntroducedPropertyInformation.FindInterfaceImplementation (typeof (object)), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_mixinIntroducedPropertyInformation.FindInterfaceDeclaration(), Is.SameAs (_declarationPropertyInformationStub));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (object));

      Assert.That (_mixinIntroducedPropertyInformation.PropertyType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void CanBeSetFromOutside ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.CanBeSetFromOutside).Return (false);

      Assert.That (_mixinIntroducedPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void GetGetMethod ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _implementationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (methodInfoAdapter);
      _declarationPropertyInformationStub.Stub (stub => stub.GetGetMethod (false)).Return (methodInfoAdapter);

      var result = _mixinIntroducedPropertyInformation.GetGetMethod (false);

      Assert.That (result, Is.TypeOf (typeof (MixinIntroducedMethodInformation)));
      Assert.That (result.Name, Is.EqualTo ("ToString"));
    }

    [Test]
    public void GetGetMethod_ReturnsNull ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetGetMethod (false)).Return (null);

      Assert.That (_mixinIntroducedPropertyInformation.GetGetMethod (false), Is.Null);
    }

    [Test]
    public void GetSetMethod ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (methodInfoAdapter);
      _declarationPropertyInformationStub.Stub (stub => stub.GetSetMethod (false)).Return (methodInfoAdapter);

      var result = _mixinIntroducedPropertyInformation.GetSetMethod (false);

      Assert.That (result, Is.TypeOf (typeof (MixinIntroducedMethodInformation)));
      Assert.That (result.Name, Is.EqualTo ("ToString"));
    }

    [Test]
    public void GetSetMethod_ReturnsNull ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (false)).Return (null);

      Assert.That (_mixinIntroducedPropertyInformation.GetSetMethod (false), Is.Null);
    }
    
    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.SetValue (instance, value, null)).WhenCalled (
          mi => instance.ImplicitInterfaceScalar = value);

      _mixinIntroducedPropertyInformation.SetValue (instance, value, null);
      
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType> ();
      var value = new SimpleReferenceType ();

      _declarationPropertyInformationStub.Stub (stub => stub.GetValue (instance, null)).Return (value);

      Assert.That(_mixinIntroducedPropertyInformation.GetValue (instance, null), Is.SameAs(value));
    }

    [Test]
    public void GetIndexParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetIndexParameters()).Return (objToReturn);

      Assert.That (_mixinIntroducedPropertyInformation.GetIndexParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void To_String ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (bool));
      Assert.That (_mixinIntroducedPropertyInformation.ToString (), Is.EqualTo ("Test(impl of 'Boolean') (Mixin)"));
    }
  }
}