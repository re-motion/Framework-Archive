using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinImplementingSimpleInterface.Method";
    }
  }
}