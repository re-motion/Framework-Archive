using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectClassWithIdentityTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void Initialize ()
    {
      BindableObjectClassWithIdentity bindableObjectClass = new BindableObjectClassWithIdentity (typeof (ClassWithIdentity), _bindableObjectProvider);

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (
          bindableObjectClass.Identifier,
          Is.EqualTo ("Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.ClassWithIdentity, Rubicon.ObjectBinding.UnitTests"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetObject_WithDefaultService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass = new BindableObjectClassWithIdentity (typeof (ClassWithIdentity), _bindableObjectProvider);
      IGetObjectService mockService = _mockRepository.CreateMock<IGetObjectService>();
      IBusinessObjectWithIdentity expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (IGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetObject_WithCustomService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass =
          new BindableObjectClassWithIdentity (typeof (ClassWithIdentityAndGetObjectServiceAttribute), _bindableObjectProvider);
      ICustomGetObjectService mockService = _mockRepository.CreateMock<ICustomGetObjectService>();
      IBusinessObjectWithIdentity expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (ICustomGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The 'Rubicon.ObjectBinding.BindableObject.IGetObjectService' required for loading objectes of type "
        + "'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.ClassWithIdentity' is not registered with the "
        + "'Rubicon.ObjectBinding.BindableObject.BindableObjectProvider'.")]
    public void GetObject_WithoutService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass = new BindableObjectClassWithIdentity (typeof (ClassWithIdentity), _bindableObjectProvider);

      bindableObjectClass.GetObject ("TheUniqueIdentifier");
    }
  }
}