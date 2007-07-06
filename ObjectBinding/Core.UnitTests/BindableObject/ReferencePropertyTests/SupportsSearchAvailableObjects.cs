using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void SearchServiceFromType_AndRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromType_AndNotRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      bool actual = property.SupportsSearchAvailableObjects (false);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromProperty", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), stubSearchServiceOnType);
      _businessObjectProvider.AddService (typeof (ISearchServiceOnProperty), mockService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Assert.That (property.SupportsSearchAvailableObjects (false), Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService ()
    {
      IBindableObjectSearchService mockService = _mockRepository.CreateMock<IBindableObjectSearchService>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService", typeof (ClassWithOtherBusinessObjectImplementation));

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (IBindableObjectSearchService), mockService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService", typeof (ClassWithOtherBusinessObjectImplementation));

      Assert.That (property.SupportsSearchAvailableObjects (false), Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), null, false, false),
          TypeFactory.GetConcreteType (propertyType));
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), null, false, false),
          propertyType);
    }
  }
}