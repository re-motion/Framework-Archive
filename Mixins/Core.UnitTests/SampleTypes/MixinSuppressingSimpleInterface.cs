using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [NotIntroduced (typeof (ISimpleInterface))]
  public class MixinSuppressingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinSuppressingSimpleInterface.Method";
    }
  }
}