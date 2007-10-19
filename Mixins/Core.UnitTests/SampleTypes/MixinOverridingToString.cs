using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinOverridingToString : Mixin<object>
  {
    [OverrideTarget]
    public new string ToString ()
    {
      return "Overridden";
    }
  }
}