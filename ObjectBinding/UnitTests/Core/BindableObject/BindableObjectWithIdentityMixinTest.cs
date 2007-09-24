using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityMixinTest : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (
          ObjectFactory.Create<ClassWithIdentity>().With ("TheUniqueIdentifier"),
          Is.InstanceOfType (typeof (IBusinessObjectWithIdentity)));
    }

    [Test]
    public void GetUniqueIdentifier ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin> (ObjectFactory.Create<ClassWithIdentity>().With ("TheUniqueIdentifier"));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That (businessObjectWithIdentity.UniqueIdentifier, Is.SameAs ("TheUniqueIdentifier"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> ().With ();
      value.String = "TheString";
      ClassWithIdentity deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> ().With ();
      byte[] serialized = Serializer.Serialize (value);
      BindableObjectProvider.SetCurrent (null);
      ClassWithIdentity deserialized = (ClassWithIdentity) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void HasMixin ()
    {
      Assert.IsTrue (BindableObjectWithIdentityMixin.HasMixin (typeof (ClassWithIdentity)));
      Assert.IsFalse (BindableObjectWithIdentityMixin.HasMixin (typeof (ClassWithAllDataTypes)));
      Assert.IsFalse (BindableObjectWithIdentityMixin.HasMixin (typeof (object)));
    }
  }
}