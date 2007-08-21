using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class BehavioralMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (NullMixin)))
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void LoadedDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (NullMixin)))
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinAddingInterface)))
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsTrue (order is IInterfaceAddedByMixin);
        Assert.AreEqual ("Hello, my ID is " + DomainObjectIDs.Order1, ((IInterfaceAddedByMixin) order).GetGreetings ());
      }
    }

    [Test]
    public void MixinCanOverrideVirtualPropertiesAndMethods ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (DOWithVirtualPropertiesAndMethods), typeof (MixinOverridingPropertiesAndMethods)))
      {
        DOWithVirtualPropertiesAndMethods instance = (DOWithVirtualPropertiesAndMethods) DomainObject.NewObject (typeof (DOWithVirtualPropertiesAndMethods));
        instance.Property = "Text";
        Assert.AreEqual ("Text-MixinSetter-MixinGetter", instance.Property);
        Assert.AreEqual ("Something-MixinMethod", instance.GetSomething ());
      }
    }
  }
}