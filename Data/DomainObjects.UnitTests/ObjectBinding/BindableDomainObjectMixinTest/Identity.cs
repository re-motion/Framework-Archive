using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
{
  [TestFixture]
  public class Identity : ObjectBindingBaseTest
  {
    [Test]
    public void BindableDomainObjectsHaveIdentity ()
    {
      BindableDomainObject domainObject = BindableDomainObject.NewObject ();
      Assert.IsTrue (domainObject is IBusinessObjectWithIdentity);
    }

    [Test]
    public void BindableDomainObjectClassesHaveIdentity ()
    {
      BindableDomainObject domainObject = BindableDomainObject.NewObject ();
      Assert.IsTrue (((IBusinessObjectWithIdentity)domainObject).BusinessObjectClass is IBusinessObjectClassWithIdentity);
    }
    
    [Test]
    public void UniqueIdentifier ()
    {
      BindableDomainObject domainObject = BindableDomainObject.NewObject ();
      Assert.AreEqual (domainObject.ID.ToString (), ((IBusinessObjectWithIdentity) domainObject).UniqueIdentifier);
    }

    [Test]
    public void GetFromUniqueIdentifier ()
    {
      BindableObjectProvider.Current.AddService (typeof (IGetBindableDomainObjectService), new GetBindableDomainObjectService());
      BindableDomainObject original = BindableDomainObject.NewObject ();
      BindableObjectClassWithIdentity boClass =
          (BindableObjectClassWithIdentity) BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableDomainObject));
      Assert.AreSame (original, boClass.GetObject (original.ID.ToString ()));
    }
  }
}
