using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectTest : ObjectBindingBaseTest
  {
    [DBTable]
    [TestDomain]
    public class SampleBindableDomainObject : BindableDomainObject
    {
    }

    [DBTable]
    [TestDomain]
    public class SampleBindableDomainObjectWithOverriddenDisplayName : BindableDomainObject
    {
      public override string DisplayName
      {
        get { return "CustomName"; }
      }
    }

    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.IsTrue (typeof (DomainObject).IsAssignableFrom (typeof (SampleBindableDomainObject)));
    }

    [Test]
    public void BindableDomainObjectAddsMixin ()
    {
      Assert.IsTrue (Mixins.TypeUtility.HasMixin (typeof (SampleBindableDomainObject), typeof (BindableDomainObjectMixin)));
    }

    [Test]
    public void DefaultDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) DomainObject.NewObject (typeof (SampleBindableDomainObject));
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject)), businessObject.DisplayName);
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) DomainObject.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName));
      Assert.AreNotEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)),
          businessObject.DisplayName);
      Assert.AreEqual ("CustomName", businessObject.DisplayName);
    }
  }
}