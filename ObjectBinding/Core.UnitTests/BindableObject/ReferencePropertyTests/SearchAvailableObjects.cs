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
  public class SearchAvailableObjects : TestBase
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
    public void Search_WithSearchSupported ()
    {
      IBusinessObject stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));
      IBusinessObject[] expected = new IBusinessObject[0];

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsIdentity (property)).Return (true);
        Expect.Call (mockService.Search (stubBusinessObject, property, "*")).Return (expected);
      }
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (stubBusinessObject, true, "*");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage =
        "Searching is not supported for reference property 'SearchServiceFromType' of business object class "
        + "'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.ClassWithSearchServiceTypeAttribute, Rubicon.ObjectBinding.UnitTests'.")]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithSearchServiceTypeAttribute>().With();
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (false);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      try
      {
        property.SearchAvailableObjects (businessObject, true, "*");
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), null, false, false),
          TypeFactory.GetConcreteType (propertyType));
    }
  }
}