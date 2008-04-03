using System;

namespace Remotion.Mixins.UnitTests.Definitions.DependencySorting.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinOverridingM1
  {
    [OverrideTarget]
    public void M1 ()
    {
    }
  }
}