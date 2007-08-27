using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [NonIntroduced (typeof (ISimpleInterface))]
  public class MixinSuppressingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinSuppressingSimpleInterface.Method";
    }
  }
}