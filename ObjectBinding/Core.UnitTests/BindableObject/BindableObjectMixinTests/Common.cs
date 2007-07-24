using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (ObjectFactory.Create<SimpleBusinessObjectClass>().With(), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass>().With();
      value.String = "TheString";
      SimpleBusinessObjectClass deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass> ().With ();
      byte[] serialized = Serializer.Serialize (value);
      BindableObjectProvider.SetCurrent (null);
      SimpleBusinessObjectClass deserialized = (SimpleBusinessObjectClass) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }
  }
}