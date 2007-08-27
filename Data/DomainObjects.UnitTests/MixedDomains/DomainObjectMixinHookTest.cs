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