using System;

namespace Remotion.Mixins.UnitTests.Definitions.DependencySorting.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinOverridingM2
  {
    [OverrideTarget]
    public void M2 ()
    {
    }
  }
}