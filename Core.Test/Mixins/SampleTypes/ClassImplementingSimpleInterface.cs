using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public class ClassImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "ClassImplementingSimpleInterface.Method";
    }
  }
}