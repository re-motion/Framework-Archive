using System;

namespace Rubicon.Mixins.UnitTests.Definitions.DependencySorting.SampleTypes
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