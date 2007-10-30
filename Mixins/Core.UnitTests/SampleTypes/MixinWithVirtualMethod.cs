using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinWithVirtualMethod : Mixin<object>
  {
    public virtual string VirtualMethod ()
    {
      return "MixinWithVirtualMethod.VirtualMethod";
    }
  }
}