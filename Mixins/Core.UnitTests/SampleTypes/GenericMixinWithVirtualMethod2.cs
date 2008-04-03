using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class GenericMixinWithVirtualMethod2<T> : Mixin<T>
      where T : class
  {
    public virtual string VirtualMethod ()
    {
      return "GenericMixinWithVirtualMethod2.VirtualMethod";
    }
  }
}