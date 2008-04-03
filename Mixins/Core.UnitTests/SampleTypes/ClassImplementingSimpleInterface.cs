using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class ClassImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "ClassImplementingSimpleInterface.Method";
    }
  }
}