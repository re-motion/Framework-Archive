using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Core.UnitTests.Globalization.SampleTypes;
using Rubicon.Globalization;

namespace Rubicon.Core.UnitTests.Globalization
{
  [TestFixture]
  public class ResourceManagerResolverUtilityTest
  {
    private MockRepository _mockRepository;
    private ResourceManagerResolverImplementation<MultiLingualResourcesAttribute> _resolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _resolverMock = _mockRepository.CreateMock<ResourceManagerResolverImplementation<MultiLingualResourcesAttribute>>();
    }

    [Test]
    public void GetResourceText ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager>();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

      string text = ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.AreEqual ("Resistance is futile", text);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetResourceText_Default ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Grob")).Return ("Grob");
      _mockRepository.ReplayAll ();

      string text = ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
      Assert.AreEqual ("", text);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ResourceException))]
    public void GetResourceText_Throw ()
    {
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Throw (new ResourceException(""));
      _mockRepository.ReplayAll ();

      ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.GetResourceText (_resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
    }

    [Test]
    public void ExistsResourceText_True ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

      bool result = ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsTrue (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Borg");
      _mockRepository.ReplayAll ();

      bool result = ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsFalse (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False_Exception ()
    {
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Throw (new ResourceException (""));
      _mockRepository.ReplayAll ();

      bool result = ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsFalse (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceTrue ()
    {
      ResourceManagerResolverImplementation<MultiLingualResourcesAttribute> resolver =
          new ResourceManagerResolverImplementation<MultiLingualResourcesAttribute>();
      Assert.IsTrue (
          ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (resolver, typeof (ClassWithMultiLingualResourcesAttributes)));
      Assert.IsTrue (
          ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (resolver, typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.IsTrue (
          ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (resolver, typeof (InheritedClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void ExistsResourceFalse ()
    {
      ResourceManagerResolverImplementation<MultiLingualResourcesAttribute> resolver =
          new ResourceManagerResolverImplementation<MultiLingualResourcesAttribute> ();
      Assert.IsFalse (
          ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (resolver, typeof (ClassWithoutMultiLingualResourcesAttributes)));
    }
  }
}