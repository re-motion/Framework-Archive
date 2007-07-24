using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Security;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetDisplayName : TestBase
  {
    private MockRepository _mockRepository;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _mockObjectSecurityAdapter = _mockRepository.CreateMock<IObjectSecurityAdapter>();
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), _mockObjectSecurityAdapter);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleBusinessObjectClass, Rubicon.ObjectBinding.UnitTests"));
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithOverriddenDisplayName>().With();

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndAccessGranted ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (true);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndWithAccessDenied ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (false);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("�"));
    }

    [Test]
    public void DisplayNameSafe_WithoutOverriddenDisplayName ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy> ();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>> ().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      _mockRepository.ReplayAll ();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll ();
      Assert.That (
          actual,
          NUnit.Framework.SyntaxHelpers.Text.StartsWith ("Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SecurableClassWithReferenceType"));
    }
  }
}