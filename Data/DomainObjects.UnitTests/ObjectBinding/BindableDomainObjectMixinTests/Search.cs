using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
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
    private IBusinessObject _orderItem;
    private IBusinessObjectReferenceProperty _property;

    private IDisposable _mixinConfiguration1;
    private IDisposable _mixinConfiguration2;

    public override void SetUp ()
    {
      base.SetUp ();
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectSearchService), new BindableDomainObjectSearchService ());

      _mixinConfiguration1 = MixinConfiguration.ScopedExtend (typeof (Order), typeof (BindableDomainObjectMixin));
      _mixinConfiguration2 = MixinConfiguration.ScopedExtend (typeof (OrderItem), typeof (BindableDomainObjectMixin));

      _orderItem = (IBusinessObject) OrderItem.NewObject();
      _property = (IBusinessObjectReferenceProperty) _orderItem.BusinessObjectClass.GetPropertyDefinition ("Order");
    }

    public override void TearDown ()
    {
      _mixinConfiguration2.Dispose ();
      _mixinConfiguration1.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void SearchServiceAttribute ()
    {
      Assert.IsTrue (_orderItem.GetType ().IsDefined (typeof (SearchAvailableObjectsServiceTypeAttribute), true));
      Assert.AreEqual (typeof (BindableDomainObjectSearchService),
          AttributeUtility.GetCustomAttribute<SearchAvailableObjectsServiceTypeAttribute> (_orderItem.GetType (), true).Type);
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects (true));
      IBusinessObjectWithIdentity[] results = (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_orderItem, true, "QueryWithSpecificCollectionType");
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects (false));
      IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, false, "QueryWithSpecificCollectionType");
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction ()
    {
      using (ClientTransaction.NewTransaction ().EnterNonReturningScope ())
      {
        IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, true, "QueryWithSpecificCollectionType");

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        Order order = (Order) results[0];
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionMock));
        Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }
    }

    [Test]
    public void SearchAvailableObjectsWithDifferentObject ()
    {
      IBusinessObject[] businessObjects =
          _property.SearchAvailableObjects ((IBusinessObject) Order.NewObject (),
          true, "QueryWithSpecificCollectionType");

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, true, null);

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, true, "");

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }
  }
}