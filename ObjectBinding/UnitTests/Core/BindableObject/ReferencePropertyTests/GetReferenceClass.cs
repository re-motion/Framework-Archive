using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class GetReferenceClass : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void UseBindableObjectProvider ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass), null, false, false),
          TypeFactory.GetConcreteType (typeof (SimpleBusinessObjectClass)));

      Assert.That (property.ReferenceClass, Is.SameAs (_businessObjectProvider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    public void UseBusinessObjectClassService ()
    {
      IBusinessObjectClassService mockService = _mockRepository.CreateMock<IBusinessObjectClassService>();
      IBusinessObjectClass expectedClass = _mockRepository.Stub<IBusinessObjectClass>();
      IBusinessObject businessObjectFromOtherBusinessObjectProvider = _mockRepository.Stub<IBusinessObject>();
      Type typeFromOtherBusinessObjectProvider = businessObjectFromOtherBusinessObjectProvider.GetType();
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeFromOtherBusinessObjectProvider);

      Expect.Call (mockService.GetBusinessObjectClass (typeFromOtherBusinessObjectProvider)).Return (expectedClass);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (IBusinessObjectClassService), mockService);
      IBusinessObjectClass actualClass = property.ReferenceClass;

      _mockRepository.VerifyAll();
      Assert.That (actualClass, Is.SameAs (expectedClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The 'Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.ClassWithOtherBusinessObjectImplementation' type does not use the "
        + "'Rubicon.ObjectBinding.BindableObject' implementation of 'Rubicon.ObjectBinding.IBusinessObject' and there is no "
        + "'Rubicon.ObjectBinding.IBusinessObjectClassService' registered with the 'Rubicon.ObjectBinding.BindableObject.BindableObjectProvider'.")]
    public void UseBusinessObjectClassService_WithoutService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassWithOtherBusinessObjectImplementation));

      Dev.Null = property.ReferenceClass;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The GetBusinessObjectClass method of 'Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.StubBusinessObjectClassService', registered "
        + "with the 'Rubicon.ObjectBinding.BindableObject.BindableObjectProvider', failed to return an 'Rubicon.ObjectBinding.IBusinessObjectClass' "
        + "for type 'Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.ClassWithOtherBusinessObjectImplementation'.")]
    public void UseBusinessObjectClassService_WithServiceReturningNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassWithOtherBusinessObjectImplementation));

      _businessObjectProvider.AddService (typeof (IBusinessObjectClassService), new StubBusinessObjectClassService());
      Dev.Null = property.ReferenceClass;
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
        GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<>).MakeGenericType (propertyType), propertyName), _businessObjectProvider),
        propertyType);
    }
  }
}