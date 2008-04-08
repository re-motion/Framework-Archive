using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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