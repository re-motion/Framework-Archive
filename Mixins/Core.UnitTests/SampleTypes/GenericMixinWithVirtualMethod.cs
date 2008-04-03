using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class GenericMixinWithVirtualMethod<T> : Mixin<T>
      where T : class
  {
    public virtual string VirtualMethod ()
    {
      return "GenericMixinWithVirtualMethod.VirtualMethod";
    }
  }
}