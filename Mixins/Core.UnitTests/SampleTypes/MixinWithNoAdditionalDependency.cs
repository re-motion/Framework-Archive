using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (TargetClassWithAdditionalDependencies))]
  public class MixinWithNoAdditionalDependency : Mixin<object, ITargetClassWithAdditionalDependencies>
  {
    [Override]
    public string GetString ()
    {
      return "MixinWithNoAdditionalDependency-" + Base.GetString ();
    }
  }
}