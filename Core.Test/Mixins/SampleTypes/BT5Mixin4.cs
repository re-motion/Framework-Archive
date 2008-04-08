using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  // no attributes
  public class BT5Mixin4
  {
    [OverrideTarget]
    public string Property
    {
      set { }
    }
  }
}