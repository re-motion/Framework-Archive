using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ReferencePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithReferenceType<SimpleClass>), _businessObjectProvider);
      _businessObjectClass = classReflector.GetMetadata();

      _mockRepository = new MockRepository();
    }

    [Test]
    public void GetReferenceClass ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.ReferenceClass, Is.SameAs (_businessObjectProvider.GetBindableObjectClass (typeof (SimpleClass))));
    }

    [Test]
    public void GetReferenceClass_FromBusinessObjectClassService ()
    {
      IBusinessObjectClassService mockService = _mockRepository.CreateMock<IBusinessObjectClassService>();
      IBusinessObjectClass expectedClass = _mockRepository.Stub<IBusinessObjectClass>();
      IBusinessObject businessObjectFromOtherBusinessObjectProvider = _mockRepository.Stub<IBusinessObject>();
      Type typeFromOtherBusinessObjectProvider = businessObjectFromOtherBusinessObjectProvider.GetType();
      Type classWithReferenceType = typeof (ClassWithReferenceType<>).MakeGenericType (typeFromOtherBusinessObjectProvider);
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (classWithReferenceType, "Scalar"), null, false),
          typeFromOtherBusinessObjectProvider);

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
        "The 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.OtherBusinessObjectImplementation' type does not use the "
        + "'Rubicon.ObjectBinding.BindableObject' implementation of 'Rubicon.ObjectBinding.IBusinessObject' and there is no "
        + "'Rubicon.ObjectBinding.IBusinessObjectClassService' registered with the 'Rubicon.ObjectBinding.BindableObject.BindableObjectProvider'.")]
    public void GetReferenceClass_FromBusinessObjectClassService_WithoutService ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<OtherBusinessObjectImplementation>), "Scalar"), null, false),
          typeof (OtherBusinessObjectImplementation));

      Dev.Null = property.ReferenceClass;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The GetBusinessObjectClass method of 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.StubBusinessObjectClassService', registered "
        + "with the 'Rubicon.ObjectBinding.BindableObject.BindableObjectProvider', failed to return an 'Rubicon.ObjectBinding.IBusinessObjectClass' "
        + "for type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.OtherBusinessObjectImplementation'.")]
    public void GetReferenceClass_FromBusinessObjectClassService_WithServiceReturningNull ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<OtherBusinessObjectImplementation>), "Scalar"), null, false),
          typeof (OtherBusinessObjectImplementation));

      _businessObjectProvider.AddService (typeof (IBusinessObjectClassService), new StubBusinessObjectClassService());
      Dev.Null = property.ReferenceClass;
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SupportsSearchAvailableObjects ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SearchAvailableObjects ()
    {
    }

    [Test]
    public void CreateIfNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.CreateIfNull, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Create method is not supported by 'Rubicon.ObjectBinding.BindableObject.ReferenceProperty'.")]
    public void Create ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      property.Create (null);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), propertyName), null, false),
          TypeFactory.GetConcreteType(typeof (SimpleClass)));
    }
  }
}