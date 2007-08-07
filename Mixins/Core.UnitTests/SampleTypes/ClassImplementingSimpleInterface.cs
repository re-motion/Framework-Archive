using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class ClassImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "ClassImplementingSimpleInterface.Method";
    }
  }
}