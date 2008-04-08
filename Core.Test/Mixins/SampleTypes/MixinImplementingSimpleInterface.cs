using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public class MixinImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinImplementingSimpleInterface.Method";
    }
  }
}