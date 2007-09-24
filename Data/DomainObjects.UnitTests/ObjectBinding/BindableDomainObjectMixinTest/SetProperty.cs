using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.Mixins;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
{
  [TestFixture]
  public class SetProperty : ObjectBindingBaseTest
  {
    private BindableDomainObject _bindableObject;
    private BindableDomainObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = BindableDomainObject.NewObject();
      _bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void WithBusinessObjectProperty ()
    {
      _businessObject.SetProperty (_businessObject.BusinessObjectClass.GetPropertyDefinition ("Name"), "James");

      Assert.That (_bindableObject.Name, Is.EqualTo ("James"));
    }

    [Test]
    public void WithPropertyIdentifier ()
    {
      _businessObject.SetProperty ("Name", "James");

      Assert.That (_bindableObject.Name, Is.EqualTo ("James"));
    }

    [Test]
    [Ignore ("TODO: test")]
    [ExpectedException (typeof (Exception), ExpectedMessage = "")]
    public void WithoutSetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableDomainObjectMixin> (BindableDomainObject.NewObject());
      businessObject.SetProperty ("Name", null);
    }
  }
}