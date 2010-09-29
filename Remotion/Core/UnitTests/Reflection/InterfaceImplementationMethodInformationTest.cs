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
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class InterfaceImplementationMethodInformationTest
  {
    private IMethodInformation _implementationMethodInformationStub;
    private IMethodInformation _declarationMethodInformationStub;
    private InterfaceImplementationMethodInformation _interfaceImplementationMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();
      _declarationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();

      _interfaceImplementationMethodInformation = new InterfaceImplementationMethodInformation (
          _implementationMethodInformationStub,
          _declarationMethodInformationStub);
    }

    [Test]
    public void Name ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_interfaceImplementationMethodInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (string));

      Assert.That (_interfaceImplementationMethodInformation.DeclaringType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (string));

      Assert.That (_interfaceImplementationMethodInformation.GetOriginalDeclaringType(), Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttribute<string> (false)).Return ("Test");

      Assert.That (_interfaceImplementationMethodInformation.GetCustomAttribute<string> (false), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttributes<string> (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.GetCustomAttributes<string> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.IsDefined<Attribute> (false)).Return (false);

      Assert.That (_interfaceImplementationMethodInformation.IsDefined<Attribute> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _implementationMethodInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (bool))).Return (methodInfoAdapter);

      Assert.That (_interfaceImplementationMethodInformation.FindInterfaceImplementation (typeof (bool)), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_interfaceImplementationMethodInformation.FindInterfaceDeclaration(), Is.SameAs (_declarationMethodInformationStub));
    }

    [Test]
    public void GetFastInvoker ()
    {
      var objToReturn = (Func<string>) (() => "Test");
      _declarationMethodInformationStub.Stub (stub => stub.GetFastInvoker (typeof (Func<string>))).Return (objToReturn);

      var invoker = _interfaceImplementationMethodInformation.GetFastInvoker<Func<string>>();

      Assert.That (invoker, Is.SameAs (objToReturn));
    }

    [Test]
    public void GetParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetParameters()).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.GetParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var objToReturn = new PropertyInfoAdapter (typeof (string).GetProperty ("Length"));
      _implementationMethodInformationStub.Stub (stub => stub.FindDeclaringProperty()).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.FindDeclaringProperty(), Is.SameAs (objToReturn));
    }

    [Test]
    public void ReturnsType ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.ReturnType).Return (typeof (bool));

      Assert.That (_interfaceImplementationMethodInformation.ReturnType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void Invoke ()
    {
      var instance = new object();
      var parameters = new object[0];
      _declarationMethodInformationStub.Stub (stub => stub.Invoke (instance, parameters)).Return ("Test");

      var result = _interfaceImplementationMethodInformation.Invoke (instance, parameters);

      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_interfaceImplementationMethodInformation.Equals (null), Is.False);
      Assert.That (_interfaceImplementationMethodInformation.Equals ("test"), Is.False);
      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _declarationMethodInformationStub)),
          Is.True);
      Assert.That (
         _interfaceImplementationMethodInformation.Equals (
             new InterfaceImplementationMethodInformation (_declarationMethodInformationStub, _declarationMethodInformationStub)),
         Is.False);
      Assert.That (
         _interfaceImplementationMethodInformation.Equals (
             new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _implementationMethodInformationStub)),
         Is.False);
      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_declarationMethodInformationStub, _implementationMethodInformationStub)),
          Is.False);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That (
          _interfaceImplementationMethodInformation.GetHashCode (),
          Is.EqualTo (new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _declarationMethodInformationStub).GetHashCode ()));
    }

    [Test]
    public void To_String ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (bool));

      Assert.That (_interfaceImplementationMethodInformation.ToString(), Is.EqualTo ("Test(impl of 'Boolean')"));
    }
  }
}