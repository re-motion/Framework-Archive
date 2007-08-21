using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Serializable]
  public class MixinAddingInterface : Mixin<DomainObject>, IInterfaceAddedByMixin
  {
    public string GetGreetings ()
    {
      return "Hello, my ID is " + This.ID;
    }
  }
}