using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
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