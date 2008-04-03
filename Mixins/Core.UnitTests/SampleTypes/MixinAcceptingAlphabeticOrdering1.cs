using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinAcceptingAlphabeticOrdering1 : Mixin<object, object>
  {
    [OverrideTarget]
    public new string ToString ()
    {
      return "MixinAcceptingAlphabeticOrdering1.ToString-" + Base.ToString ();
    }
  }
}