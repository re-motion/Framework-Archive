using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (MixinAcceptingAlphabeticOrdering1))]
  [Uses (typeof (MixinAcceptingAlphabeticOrdering2))]
  public class ClassWithMixinsAcceptingAlphabeticOrdering
  {
    public override string ToString ()
    {
      return "ClassWithMixinsAcceptingAlphabeticOrdering.ToString";
    }
  }
}