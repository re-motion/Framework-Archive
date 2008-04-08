using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public class MixinWithVirtualMethod : Mixin<object>
  {
    public virtual string VirtualMethod ()
    {
      return "MixinWithVirtualMethod.VirtualMethod";
    }
  }
}