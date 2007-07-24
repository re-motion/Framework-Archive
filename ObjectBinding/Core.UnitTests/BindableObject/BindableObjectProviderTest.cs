using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest : TestBase
  {
    private BindableObjectProvider _provider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.That (BindableObjectProvider.Current, Is.TypeOf (typeof (BindableObjectProvider)));
      Assert.That (
          BindableObjectProvider.Current.GetService<IBindableObjectGlobalizationService>(),
          Is.TypeOf (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetInstance_SameTwice ()
    {
      Assert.That (BindableObjectProvider.Current, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void SetInstance ()
    {
      BindableObjectProvider.SetCurrent (_provider);
      Assert.That (BindableObjectProvider.Current, Is.SameAs (_provider));
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
      _provider.AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService>());

      Assert.That (_provider.GetService<IBusinessObjectService>(), Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetDefaultServices ()
    {
      IBusinessObjectProvider currentProvider = BindableObjectProvider.Current;

      Assert.That (
        currentProvider.GetService (typeof (IBusinessObjectStringFormatterService)), 
        Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));

      Assert.That (
        currentProvider.GetService (typeof (IBindableObjectGlobalizationService)),
        Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      BindableObjectClass outValue;
      Assert.That (_provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleBusinessObjectClass), out outValue), Is.False);

      BindableObjectClass actual = _provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (_provider));
      BindableObjectClass cachedBindableObjectClass;
      Assert.That (_provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleBusinessObjectClass), out cachedBindableObjectClass), Is.True);
      Assert.That (actual, Is.SameAs (cachedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass)), Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       ExpectedMessage =
       "Type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
       + "'Rubicon.ObjectBinding.IBusinessObject' interface via the 'Rubicon.ObjectBinding.BindableObject.BindableObjectMixin'.\r\n"
       + "Parameter name: type")]
    public void GetBindableObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _provider.GetBindableObjectClass (typeof (SimpleReferenceType));
    }
  }
}