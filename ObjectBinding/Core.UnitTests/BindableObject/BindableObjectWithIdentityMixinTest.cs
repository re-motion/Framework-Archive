using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
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
  }
}