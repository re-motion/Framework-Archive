using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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