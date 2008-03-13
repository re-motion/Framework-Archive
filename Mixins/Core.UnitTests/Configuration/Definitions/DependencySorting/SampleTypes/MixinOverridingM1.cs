using System;

namespace Rubicon.Mixins.UnitTests.Configuration.Definitions.DependencySorting.SampleTypes
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