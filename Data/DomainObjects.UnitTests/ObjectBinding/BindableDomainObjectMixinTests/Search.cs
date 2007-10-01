using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class Search : ObjectBindingBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectSearchService), new BindableDomainObjectSearchService ());
    }

    [Test]
    public void SearchServiceAttribute ()
    {
      object domainObject = BindableSampleDomainObject.NewObject ();
      Assert.IsTrue (domainObject.GetType ().IsDefined (typeof (SearchAvailableObjectsServiceTypeAttribute), true));
      Assert.AreEqual (typeof (BindableDomainObjectSearchService),
          AttributeUtility.GetCustomAttribute<SearchAvailableObjectsServiceTypeAttribute> (domainObject.GetType (), true).Type);
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (BindableDomainObjectMixin)))
      using (MixinConfiguration.ScopedExtend (typeof (OrderItem), typeof (BindableDomainObjectMixin)))
      {
        IBusinessObject orderItem = (IBusinessObject) OrderItem.NewObject();
        IBusinessObjectReferenceProperty property = (IBusinessObjectReferenceProperty) orderItem.BusinessObjectClass.GetPropertyDefinition ("Order");
        Assert.IsTrue (property.SupportsSearchAvailableObjects (true));
        IBusinessObject[] results = property.SearchAvailableObjects (orderItem, true, "QueryWithSpecificCollectionType");
        Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
        foreach (IBusinessObject obj in results)
          Assert.IsTrue (obj is IBusinessObjectWithIdentity);
      }
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (BindableDomainObjectMixin)))
      using (MixinConfiguration.ScopedExtend (typeof (OrderItem), typeof (BindableDomainObjectMixin)))
      {
        IBusinessObject orderItem = (IBusinessObject) OrderItem.NewObject ();
        IBusinessObjectReferenceProperty property = (IBusinessObjectReferenceProperty) orderItem.BusinessObjectClass.GetPropertyDefinition ("Order");
        Assert.IsTrue (property.SupportsSearchAvailableObjects (false));
        IBusinessObject[] results = property.SearchAvailableObjects (orderItem, false, "QueryWithSpecificCollectionType");
        Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
      }
    }
  }
}