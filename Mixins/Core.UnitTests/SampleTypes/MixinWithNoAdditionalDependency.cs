using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (TargetClassWithAdditionalDependencies))]
  public class MixinWithNoAdditionalDependency : Mixin<object, ITargetClassWithAdditionalDependencies>
  {
    [OverrideTarget]
    public string GetString ()
    {
      return "MixinWithNoAdditionalDependency-" + Base.GetString ();
    }
  }
}