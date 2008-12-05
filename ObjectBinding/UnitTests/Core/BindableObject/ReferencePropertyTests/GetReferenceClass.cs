// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class GetReferenceClass : TestBase
  {
    private MockRepository _mockRepository;
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectProvider _bindableObjectWithIdentityProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _bindableObjectProvider = new BindableObjectProvider ();
      _bindableObjectWithIdentityProvider = new BindableObjectProvider ();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectWithIdentityProvider);
    }

    [Test]
    public void UseBindableObjectProvider ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceType<ClassWithIdentity>), "Scalar"),
              typeof (ClassWithIdentity),
              null,
              false,
              false),
          TypeFactory.GetConcreteType (typeof (ClassWithIdentity)));

      Assert.That (property.ReferenceClass, Is.SameAs (BindableObjectProvider.GetBindableObjectClass (typeof (ClassWithIdentity))));
      Assert.That (
          property.BusinessObjectProvider,
          Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType(typeof (ClassWithReferenceType<ClassWithIdentity>))));
      Assert.That (
          property.ReferenceClass.BusinessObjectProvider,
          Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassWithIdentity))));
      Assert.That (property.ReferenceClass.BusinessObjectProvider, Is.Not.SameAs (property.BusinessObjectProvider));
    }

    [Test]
    public void UseBindableObjectProvider_WithBaseClass ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceToClassDerivedFromBindableObjectBase), "ScalarReference"),
              typeof (ClassDerivedFromBindableObjectBase),
              null,
              false,
              false),
          typeof (ClassDerivedFromBindableObjectBase));

      Assert.That (property.ReferenceClass, Is.SameAs (BindableObjectProvider.GetBindableObjectClass (typeof (ClassDerivedFromBindableObjectBase))));
      Assert.That (
          property.BusinessObjectProvider,
          Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassWithReferenceToClassDerivedFromBindableObjectBase))));
      Assert.That (
          property.ReferenceClass.BusinessObjectProvider,
          Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectBase))));
      Assert.That (property.ReferenceClass.BusinessObjectProvider, Is.SameAs (property.BusinessObjectProvider));
    }

    [Test]
    public void UseBusinessObjectClassService ()
    {
      IBusinessObjectClassService mockService = _mockRepository.StrictMock<IBusinessObjectClassService>();
      IBusinessObjectClass expectedClass = _mockRepository.Stub<IBusinessObjectClass>();
      IBusinessObject businessObjectFromOtherBusinessObjectProvider = _mockRepository.Stub<IBusinessObject>();
      Type typeFromOtherBusinessObjectProvider = businessObjectFromOtherBusinessObjectProvider.GetType();
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeFromOtherBusinessObjectProvider);

      Expect.Call (mockService.GetBusinessObjectClass (typeFromOtherBusinessObjectProvider)).Return (expectedClass);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (IBusinessObjectClassService), mockService);
      IBusinessObjectClass actualClass = property.ReferenceClass;

      _mockRepository.VerifyAll();
      Assert.That (actualClass, Is.SameAs (expectedClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassFromOtherBusinessObjectImplementation' type does not use the "
        + "'Remotion.ObjectBinding.BindableObject' implementation of 'Remotion.ObjectBinding.IBusinessObject' and there is no "
        + "'Remotion.ObjectBinding.IBusinessObjectClassService' registered with the 'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type.")]
    public void UseBusinessObjectClassService_WithoutService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassFromOtherBusinessObjectImplementation));

      Dev.Null = property.ReferenceClass;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The GetBusinessObjectClass method of 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.StubBusinessObjectClassService', registered "
        + "with the 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider', failed to return an 'Remotion.ObjectBinding.IBusinessObjectClass' "
        + "for type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassFromOtherBusinessObjectImplementation'.")]
    public void UseBusinessObjectClassService_WithServiceReturningNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassFromOtherBusinessObjectImplementation));

      _bindableObjectProvider.AddService (typeof (IBusinessObjectClassService), new StubBusinessObjectClassService());
      Dev.Null = property.ReferenceClass;
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
        GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<>).MakeGenericType (propertyType), propertyName), _bindableObjectProvider),
        propertyType);
    }
  }
}
