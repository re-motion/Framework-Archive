using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectMixinTest : TestBase
  {
    private SimpleBusinessObjectClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _bindableObject = ObjectFactory.Create<SimpleBusinessObjectClass> ().With ();
      _bindableObjectMixin = Mixin.Get<BindableObjectMixin> (_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (_bindableObject, Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      Assert.That (_bindableObjectMixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (_bindableObjectMixin.BusinessObjectClass.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (_bindableObjectMixin.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void GetBusinessObjectClass_FromInterface ()
    {
      Assert.That (_businessObject.BusinessObjectClass, Is.Not.Null);
      Assert.That (_businessObject.BusinessObjectClass, Is.SameAs (_bindableObjectMixin.BusinessObjectClass));
      Assert.That (_businessObject.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void GetProperty_WithBusinessObjectProperty ()
    {
      _bindableObject.String = "A String";

      Assert.That (_businessObject.GetProperty (_businessObject.BusinessObjectClass.GetPropertyDefinition ("String")), Is.EqualTo ("A String"));
    }

    [Test]
    public void GetProperty_WithPropertyIdentifier ()
    {
      _bindableObject.String = "A String";

      Assert.That (_businessObject.GetProperty ("String"), Is.EqualTo ("A String"));
    }

    [Test]
    [Ignore ("TODO: test")]
    [ExpectedException (typeof (Exception), ExpectedMessage = "")]
    public void GetProperty_WithoutGetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());
      businessObject.GetProperty ("String");
    }

    [Test]
    public void SetProperty_WithPropertyIdentifier ()
    {
      _businessObject.SetProperty ("String", "A String");

      Assert.That (_bindableObject.String, Is.EqualTo ("A String"));
    }

    [Test]
    [Ignore ("TODO: test")]
    [ExpectedException (typeof (Exception), ExpectedMessage = "")]
    public void GetProperty_WithoutSetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());
      businessObject.SetProperty ("String", null);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetPropertyString ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SerializeAndDeserialize ()
    {
    }
  }
}