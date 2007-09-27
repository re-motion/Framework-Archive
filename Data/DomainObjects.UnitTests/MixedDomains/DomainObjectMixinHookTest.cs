using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinHookTest : ClientTransactionBaseTest
  {
    [Test]
    public void OnDomainObjectLoaded ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (HookedDomainObjectMixin)))
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);

        using (new MixedTypeInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedAfterEnlist ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (HookedDomainObjectMixin)))
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);

        Order order;
        using (new MixedTypeInstantiationScope (mixinInstance))
        {
          order = Order.GetObject (DomainObjectIDs.Order1);
        }

        mixinInstance.OnLoadedCalled = false;
        mixinInstance.OnLoadedCount = 0;

        ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
        newTransaction.EnlistDomainObject (order);

        Assert.IsFalse (mixinInstance.OnLoadedCalled);

        using (newTransaction.EnterScope ())
        {
          ++order.OrderNumber;
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.AreEqual (1, mixinInstance.OnLoadedCount);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedInSubTransaction ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (HookedDomainObjectMixin)))
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);

        using (new MixedTypeInstantiationScope (mixinInstance))
        {
          using (ClientTransactionMock.CreateSubTransaction().EnterScope())
          {
            Order.GetObject (DomainObjectIDs.Order1);
          }
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (2, mixinInstance.OnLoadedCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedInParentAndSubTransaction ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (HookedDomainObjectMixin)))
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);

        using (new MixedTypeInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
          Assert.IsTrue (mixinInstance.OnLoadedCalled);
          Assert.AreEqual (1, mixinInstance.OnLoadedCount);
          Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);

          using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
          {
            Order.GetObject (DomainObjectIDs.Order1);
          }
        }

        Assert.AreEqual (2, mixinInstance.OnLoadedCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
      }
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (HookedDomainObjectMixin)))
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);

        using (new MixedTypeInstantiationScope (mixinInstance))
        {
          Order.NewObject ();
        }

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsTrue (mixinInstance.OnCreatedCalled);
      }
    }
  }
}