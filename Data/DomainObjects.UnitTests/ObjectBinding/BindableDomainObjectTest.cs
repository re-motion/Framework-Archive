using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectTest
  {
    [DBTable]
    [TestDomain]
    public class SampleBindableDomainObject : BindableDomainObject
    {
    }

    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.IsTrue (typeof (DomainObject).IsAssignableFrom (typeof (SampleBindableDomainObject)));
    }

    [Test]
    public void BindableDomainObjectAddsMixin()
    {
      Assert.IsTrue (Mixins.TypeUtility.HasMixin (typeof (SampleBindableDomainObject), typeof (BindableDomainObjectMixin)));
    }
  }
}