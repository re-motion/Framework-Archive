using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Mixins.Validation;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinBaseClassTest : ClientTransactionBaseTest
  {
    private Order _loadedOrder;
    private Order _newOrder;
    private MixinWithAccessToDomainObjectProperties<Order> _loadedOrderMixin;
    private MixinWithAccessToDomainObjectProperties<Order> _newOrderMixin;

    public override void SetUp ()
    {
      base.SetUp ();
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinWithAccessToDomainObjectProperties<>)))
      {
        _loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
        _newOrder = Order.NewObject ();
        _loadedOrderMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<Order>> (_loadedOrder);
        _newOrderMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<Order>> (_newOrder);
      }
    }

    [Test]
    public void MixinIsApplied ()
    {
      Assert.IsNotNull (_loadedOrderMixin);
      Assert.IsNotNull (_newOrderMixin);
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void InvalidMixinConfiguration ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinWithAccessToDomainObjectProperties<Official>)))
      {
        Order.NewObject ();
      }
    }

    [Test]
    public void This ()
    {
      Assert.AreSame (_loadedOrder, _loadedOrderMixin.This);
      Assert.AreSame (_newOrder, _newOrderMixin.This);
    }

    [Test]
    public void ID ()
    {
      Assert.AreSame (_loadedOrder.ID, _loadedOrderMixin.ID);
      Assert.AreSame (_newOrder.ID, _newOrderMixin.ID);
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Assert.AreSame (_loadedOrder.GetPublicDomainObjectType (), _loadedOrderMixin.GetPublicDomainObjectType ());
      Assert.AreSame (_newOrder.GetPublicDomainObjectType (), _newOrderMixin.GetPublicDomainObjectType ());
    }

    [Test]
    public void State ()
    {
      Assert.AreEqual (_loadedOrder.State, _loadedOrderMixin.State);
      Assert.AreEqual (_newOrder.State, _newOrderMixin.State);

      ++_loadedOrder.OrderNumber;
      Assert.AreEqual (_loadedOrder.State, _loadedOrderMixin.State);

      _loadedOrder.Delete ();
      Assert.AreEqual (_loadedOrder.State, _loadedOrderMixin.State);
    }

    [Test]
    public void IsDiscarded()
    {
      Assert.AreEqual (_loadedOrder.IsDiscarded, _loadedOrderMixin.IsDiscarded);
      Assert.AreEqual (_newOrder.IsDiscarded, _newOrderMixin.IsDiscarded);

      _newOrder.Delete ();

      Assert.AreEqual (_newOrder.IsDiscarded, _newOrderMixin.IsDiscarded);
    }

    [Test]
    public void Properties ()
    {
      Assert.AreEqual (_loadedOrder.Properties, _loadedOrderMixin.Properties);
      Assert.AreEqual (_newOrder.Properties, _newOrderMixin.Properties);
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      Assert.IsFalse (_loadedOrderMixin.OnDomainObjectCreatedCalled);
      Assert.IsTrue (_newOrderMixin.OnDomainObjectCreatedCalled);
    }

    [Test]
    public void OnDomainObjectLoaded ()
    {
      Assert.IsTrue (_loadedOrderMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, _loadedOrderMixin.OnDomainObjectLoadedLoadMode);

      _loadedOrderMixin.OnDomainObjectLoadedCalled = false;
      using (ClientTransaction.NewTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (_loadedOrder);
        ++_loadedOrder.OrderNumber;
      }

      Assert.IsTrue (_loadedOrderMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.DataContainerLoadedOnly, _loadedOrderMixin.OnDomainObjectLoadedLoadMode);

      Assert.IsFalse (_newOrderMixin.OnDomainObjectLoadedCalled);
    }
  }
}