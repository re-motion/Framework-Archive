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
  public class GetProperty : ObjectBindingBaseTest
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
      _bindableObject.Name = "Earl";

      Assert.That (_businessObject.GetProperty (_businessObject.BusinessObjectClass.GetPropertyDefinition ("Name")), Is.EqualTo ("Earl"));
    }

    [Test]
    public void WithPropertyIdentifier ()
    {
      _bindableObject.Name = "Earl";

      Assert.That (_businessObject.GetProperty ("Name"), Is.EqualTo ("Earl"));
    }

    [Test]
    [Ignore ("TODO: test")]
    [ExpectedException (typeof (Exception), ExpectedMessage = "")]
    public void WithoutGetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin> (BindableDomainObject.NewObject());
      businessObject.GetProperty ("Name");
    }
  }
}