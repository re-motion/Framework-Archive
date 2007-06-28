using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectMixinTest
  {
    private SimpleClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    [SetUp]
    public void SetUp ()
    {
      _bindableObject = ObjectFactory.Create<SimpleClass>().With();
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
      Assert.That (_bindableObjectMixin.BusinessObjectClass.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (_bindableObjectMixin.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Instance));
    }

    [Test]
    public void GetBusinessObjectClass_FromInterface ()
    {
      Assert.That (_businessObject.BusinessObjectClass, Is.Not.Null);
      Assert.That (_businessObject.BusinessObjectClass, Is.SameAs (_bindableObjectMixin.BusinessObjectClass));
      Assert.That (_businessObject.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Instance));
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
    public void SetProperty_WithPropertyIdentifier ()
    {
      _businessObject.SetProperty ("String", "A String");

      Assert.That ( _bindableObject.String, Is.EqualTo ("A String"));
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