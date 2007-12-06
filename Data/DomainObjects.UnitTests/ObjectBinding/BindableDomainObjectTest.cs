using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;
using Rubicon.ObjectBinding;
using TypeUtility=Rubicon.Utilities.TypeUtility;

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
    [Serializable]
    public class SampleBindableDomainObjectWithOverriddenDisplayName : BindableDomainObject
    {
      private int _test;

      [StorageClassNone]
      public int Test
      {
        get { return _test; }
        set { _test = value; }
      }

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
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObject)).With();
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject)), businessObject.DisplayName);
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      Assert.AreNotEqual (
          TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)),
          businessObject.DisplayName);
      Assert.AreEqual ("CustomName", businessObject.DisplayName);
    }

    [Test]
    public void VerifyInterfaceImplementation ()
    {
      IBusinessObjectWithIdentity businessObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      IBusinessObjectWithIdentity businessObjectMixin = Mixin.Get<BindableDomainObjectMixin> (businessObject);

      Assert.AreSame (businessObjectMixin.BusinessObjectClass, businessObject.BusinessObjectClass);
      Assert.AreEqual (businessObjectMixin.DisplayName, businessObject.DisplayName);
      Assert.AreEqual (businessObjectMixin.DisplayNameSafe, businessObject.DisplayNameSafe);
      businessObject.SetProperty ("Test", 1);
      Assert.AreEqual (1, businessObject.GetProperty ("Test"));
      Assert.AreEqual (1, businessObject.GetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test")));
      Assert.AreEqual ("001", businessObject.GetPropertyString (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test"), "000"));
      Assert.AreEqual ("1", businessObject.GetPropertyString ("Test"));
      Assert.AreEqual (businessObjectMixin.UniqueIdentifier, businessObject.UniqueIdentifier);
      businessObject.SetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test"), 2);
      Assert.AreEqual (2, businessObject.GetProperty ("Test"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SampleBindableDomainObjectWithOverriddenDisplayName domainObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();

      Serializer.SerializeAndDeserialize (domainObject);
    }
  }
}