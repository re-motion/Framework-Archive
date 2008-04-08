using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.Definitions.DependencySorting.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinOverridingM1M2
  {
    [OverrideTarget]
    public void M1 ()
    {
    }

    [OverrideTarget]
    public void M2 ()
    {
    }
  }
}