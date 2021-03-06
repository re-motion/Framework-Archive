// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseTest
  {
    private ClassDerivedFromBindableObjectWithIdentityBase _instance;
    private IBindableObjectWithIdentityBaseImplementation _implementationMock;
    private IBusinessObjectProperty _propertyFake;
    private IBusinessObjectClass _businessObjectClassFake;

    [SetUp]
    public void SetUp()
    {
      _implementationMock = MockRepository.GenerateMock<IBindableObjectWithIdentityBaseImplementation> ();
      _instance = new ClassDerivedFromBindableObjectWithIdentityBase (_implementationMock);

      _propertyFake = MockRepository.GenerateMock<IBusinessObjectProperty> ();
      _businessObjectClassFake = MockRepository.GenerateMock<IBusinessObjectClass> ();
    }

    [Test]
    public void BindableObjectProviderAttribute()
    {
      Assert.That (typeof (BindableObjectWithIdentityBase).IsDefined (typeof (BindableObjectWithIdentityProviderAttribute), false), Is.True);
    }

    [Test]
    public void BindableObjectBaseClassAttribute ()
    {
      Assert.That (typeof (BindableObjectWithIdentityBase).IsDefined (typeof (BindableObjectBaseClassAttribute), false), Is.True);
    }

    [Test]
    public void CreateImplementation ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      Assert.That (PrivateInvoke.GetNonPublicField (instance, "_implementation"), Is.InstanceOf (typeof (BindableObjectWithIdentityBaseImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase();
      var mixin = (BindableObjectWithIdentityBaseImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Serialization ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      instance = Serializer.SerializeAndDeserialize (instance);
      var mixin = (BindableObjectWithIdentityBaseImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void GetProperty()
    {
      _implementationMock.Expect (mock => mock.GetProperty (_propertyFake)).Return (12);
      _implementationMock.Replay ();

      Assert.That (_instance.GetProperty (_propertyFake), Is.EqualTo (12));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetProperty ()
    {
      _implementationMock.Expect (mock => mock.SetProperty (_propertyFake, 174));
      _implementationMock.Replay ();

      _instance.SetProperty (_propertyFake, 174);
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetPropertyString()
    {
      _implementationMock.Expect (mock => mock.GetPropertyString (_propertyFake, "gj")).Return ("yay");
      _implementationMock.Replay ();

      Assert.That (_instance.GetPropertyString (_propertyFake, "gj"), Is.EqualTo ("yay"));
      _implementationMock.VerifyAllExpectations (); 
    }

    [Test]
    public void DisplayName()
    {
      _implementationMock.Expect (mock => mock.BaseDisplayName).Return ("Philips");
      _implementationMock.Replay ();

      Assert.That (_instance.DisplayName, Is.EqualTo ("Philips"));
      _implementationMock.VerifyAllExpectations (); 
    }

    [Test]
    public void BusinessObjectClass ()
    {
      _implementationMock.Expect (mock => mock.BusinessObjectClass).Return (_businessObjectClassFake);
      _implementationMock.Replay ();

      Assert.That (_instance.BusinessObjectClass, Is.SameAs (_businessObjectClassFake));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void UniqueIdentifier ()
    {
      _instance.SetUniqueIdentifier("uniqueID");
      Assert.That (_instance.UniqueIdentifier, Is.EqualTo ("uniqueID"));
    }
  }
}
