using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Rhino.Mocks;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest : TestBase
  {
    private BindableObjectProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new BindableObjectProvider();
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithIdentityType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassWithIdentity));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectWithIdentityProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }

    [Test]
    [Ignore ("TODO: Implement once AttributeUtility has been extended.")]
    public void GetProviderForBindableObjectType_WithAttributeFromTypeOverridingAttributeFromMixin ()
    {
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.ManualBusinessObject' does not have the "
        + "'Remotion.ObjectBinding.BusinessObjectProviderAttribute' applied.\r\nParameter name: type")]
    public void GetProviderForBindableObjectType_WithMissingAttributeType ()
    {
      BindableObjectProvider.GetProviderForBindableObjectType (typeof (ManualBusinessObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The business object provider associated with the type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.StubBusinessObject' "
        + "is not of type 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider'.\r\nParameter name: type")]
    public void GetProviderForBindableObjectType_WithInvalidProviderType ()
    {
      BindableObjectProvider.GetProviderForBindableObjectType (typeof (StubBusinessObject));
    }

    [Test]
    public void GetDefaultServices ()
    {
      Assert.That (_provider.GetService (typeof (IBusinessObjectStringFormatterService)), Is.Null);
      Assert.That (_provider.GetService (typeof (IBindableObjectGlobalizationService)), Is.Null);

      ((IBusinessObjectProvider) _provider).InitializeDefaultServices();

      Assert.That (
          _provider.GetService (typeof (IBusinessObjectStringFormatterService)), Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));
      Assert.That (
          _provider.GetService (typeof (IBindableObjectGlobalizationService)), Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      MockRepository mockRepository = new MockRepository();
      IMetadataFactory metadataFactoryMock = mockRepository.CreateMock<IMetadataFactory>();
      IClassReflector classReflectorMock = mockRepository.CreateMock<IClassReflector>();

      BindableObjectProvider provider = new BindableObjectProvider (metadataFactoryMock);
      Type targetType = typeof (SimpleBusinessObjectClass);
      Type concreteType = Mixins.TypeUtility.GetConcreteMixedType (targetType);
      BindableObjectClass expectedBindableObjectClass = new BindableObjectClass (concreteType, provider);

      Expect.Call (metadataFactoryMock.CreateClassReflector (targetType, provider)).Return (classReflectorMock);
      Expect.Call (classReflectorMock.GetMetadata()).Return (expectedBindableObjectClass);

      mockRepository.ReplayAll();

      BindableObjectClass actual = provider.GetBindableObjectClass (targetType);

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (expectedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That (
          _provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass)),
          Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding.BindableObject.BindableObjectMixinBase`1'.\r\n"
        + "Parameter name: concreteType")]
    public void GetBindableObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _provider.GetBindableObjectClass (typeof (SimpleReferenceType));
    }

    [Test]
    public void GetBindableObjectClassFromProvider ()
    {
      BindableObjectClass actual = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (SimpleBusinessObjectClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.TargetType, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (
          actual,
          Is.SameAs (
              BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)).GetBindableObjectClass (
                  typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    public void GetMetadataFactory_WithDefaultFactory ()
    {
      Assert.AreSame (DefaultMetadataFactory.Instance, _provider.MetadataFactory);
    }

    [Test]
    public void GetMetadataFactoryForType_WithCustomMetadataFactory ()
    {
      IMetadataFactory metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      Assert.AreSame (metadataFactoryStub, new BindableObjectProvider (metadataFactoryStub).MetadataFactory);
    }
  }
}