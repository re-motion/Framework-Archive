using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
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
    }

    [Test]
    public void GetReferenceClass ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.ReferenceClass, Is.SameAs (_businessObjectProvider.GetBindableObjectClass (typeof (SimpleClass))));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetReferenceClass_FromBusinessObjectClassService ()
    {
      //IBusinessObjectReferenceProperty property = new ReferenceProperty (new PropertyBase.Parameters (
      //    _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<ClassFromOtherBusinessObjectProvider>), "Scalar"), null, false));

      //IBusinessObjectClassService mockService = _mockRepository.CreateMock<IBusinessObjectClassService>();
      //IBusinessObjectClass expectedClass = _mockRepository.CreateMock<IBusinessObjectClass>();
      //Expect.Call ((Type) mockService.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectProvider))).Return (expectedClass);
      //_mockRepository.ReplayAll();

      //_businessObjectProvider.AddService (typeof (IBusinessObjectClassService), mockService);
      //IBusinessObjectClass actualClass = property.ReferenceClass;

      //_mockRepository.VerifyAll();
      //Assert.That (actualClass, Is.SameAs (expectedClass));
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
      return new ReferenceProperty (new PropertyBase.Parameters (
          _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), propertyName), null, false));
    }
  }
}