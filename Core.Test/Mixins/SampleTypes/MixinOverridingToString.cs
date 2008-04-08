using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public class MixinOverridingToString : Mixin<object, object>
  {
    [OverrideTarget]
    public new string ToString ()
    {
      return "Overridden: " + Base.ToString();
    }
  }
}