using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetProperty : TestBase
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
    public void WithBusinessObjectProperty ()
    {
      _bindableObject.String = "A String";

      Assert.That (_businessObject.GetProperty (_businessObject.BusinessObjectClass.GetPropertyDefinition ("String")), Is.EqualTo ("A String"));
    }

    [Test]
    public void WithPropertyIdentifier ()
    {
      _bindableObject.String = "A String";

      Assert.That (_businessObject.GetProperty ("String"), Is.EqualTo ("A String"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "The property 'StringWithoutGetter' was not found on business object class "
        + "'Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleBusinessObjectClass, Rubicon.ObjectBinding.UnitTests'.")]
    [Ignore ("TODO: discuss desired behavior")]
    public void WithoutGetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());
      businessObject.GetProperty ("StringWithoutGetter");
    }
  }
}