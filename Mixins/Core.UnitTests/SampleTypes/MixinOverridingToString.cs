using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
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