using System;

namespace Rubicon.Mixins.UnitTests.Definitions.DependencySorting.SampleTypes
{
  public class MixinWithBaseCallDependency2OverridingM1 : Mixin<object, IBaseCallDependency2>
  {
    [OverrideTarget]
    public void M1 ()
    {
    }
  }
}