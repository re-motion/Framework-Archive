using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class ClassWithMixedMixin
  {
    public virtual string StringMethod (int i)
    {
      return "ClassWithMixedMixin.StringMethod (" + i + ")";
    }
  }
}