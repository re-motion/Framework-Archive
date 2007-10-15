using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class ClassWithMixedMixin
  {
    public virtual string StringMethod (int i)
    {
      return "ClassWithMixedMixin.StringMethod (" + i + ")";
    }
  }
}