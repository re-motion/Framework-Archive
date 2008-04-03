using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class MixinWithVirtualMethod2 : Mixin<object>
  {
    public virtual string VirtualMethod ()
    {
      return "MixinWithVirtualMethod2.VirtualMethod";
    }
  }
}