using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinAcceptingAlphabeticOrdering2 : Mixin<object, object>
  {
    [OverrideTarget]
    public new string ToString ()
    {
      return "MixinAcceptingAlphabeticOrdering2.ToString-" + Base.ToString ();
    }
  }
}