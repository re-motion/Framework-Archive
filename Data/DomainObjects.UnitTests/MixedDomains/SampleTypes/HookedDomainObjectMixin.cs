using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [NonIntroduced (typeof (IDomainObjectMixin))]
  public class HookedDomainObjectMixin : Mixin<Order>, IDomainObjectMixin
  {
    public bool OnLoadedCalled = false;
    public bool OnCreatedCalled = false;

    public void OnDomainObjectLoaded ()
    {
      OnLoadedCalled = true;
      Assert.IsNotNull (This.ID);
      ++This.OrderNumber;
      Assert.IsNotNull (This.OrderItems);
    }

    public void OnDomainObjectCreated ()
    {
      OnCreatedCalled = true;
      Assert.IsNotNull (This.ID);
      This.OrderNumber += 2;
      Assert.IsNotNull (This.OrderItems);
    }
  }
}