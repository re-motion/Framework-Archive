using System;

namespace Rubicon.Mixins.UnitTests.Configuration.Definitions.DependencySorting.SampleTypes
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