using System;
using Rubicon.Mixins;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableObjectMixinTest
  {
    private SimpleClass _simpleClass;

    [SetUp]
    public void SetUp ()
    {
      _simpleClass = ObjectFactory.Create<SimpleClass>().With();
    }

    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (_simpleClass, Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      IBusinessObjectClass businessObjectClass = ((IBusinessObject) _simpleClass).BusinessObjectClass;

      Assert.That (businessObjectClass, Is.Not.Null);
      Assert.That (businessObjectClass, Is.InstanceOfType (typeof (BindableObjectClass)));
      Assert.That (((BindableObjectClass) businessObjectClass).Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (businessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Instance));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetProperty ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetPropertyString ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SetProperty ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SerializeAndDeserialize ()
    {
    }
  }
}