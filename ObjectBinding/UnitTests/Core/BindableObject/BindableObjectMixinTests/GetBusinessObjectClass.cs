using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetBusinessObjectClass : TestBase
  {
    private SimpleBusinessObjectClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = ObjectFactory.Create<SimpleBusinessObjectClass>().With();
      _bindableObjectMixin = Mixin.Get<BindableObjectMixin> (_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void FromClass ()
    {
      Assert.That (_bindableObjectMixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (_bindableObjectMixin.BusinessObjectClass.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (_bindableObjectMixin.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void FromInterface ()
    {
      Assert.That (_businessObject.BusinessObjectClass, Is.Not.Null);
      Assert.That (_businessObject.BusinessObjectClass, Is.SameAs (_bindableObjectMixin.BusinessObjectClass));
      Assert.That (_businessObject.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }
  }
}