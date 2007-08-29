using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinOverridingToString : Mixin<object>
  {
    [Override]
    public new string ToString ()
    {
      return "Overridden";
    }
  }
}