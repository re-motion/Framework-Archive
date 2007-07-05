using System;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest
  {
    private BindableObjectProvider _provider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _provider = new BindableObjectProvider ();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.That (BindableObjectProvider.Instance, Is.InstanceOfType (typeof (BindableObjectProvider)));
    }

    [Test]
    public void GetInstance_SameTwice ()
    {
      Assert.That (BindableObjectProvider.Instance, Is.SameAs (BindableObjectProvider.Instance));
    }

    [Test]
    public void AddAndGetService ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      _provider.AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void GetServiceFromGeneric ()
    {
      _provider.AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService> ());

      Assert.That (_provider.GetService<IBusinessObjectService>(), Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      BindableObjectClass outValue;
      Assert.That (_provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleClass), out outValue), Is.False);

      BindableObjectClass actual = _provider.GetBindableObjectClass (typeof (SimpleClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (_provider));
      BindableObjectClass cachedBindableObjectClass;
      Assert.That (_provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleClass), out cachedBindableObjectClass), Is.True);
      Assert.That (actual, Is.SameAs (cachedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That (_provider.GetBindableObjectClass (typeof (SimpleClass)), Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleClass))));
    }
  }
}