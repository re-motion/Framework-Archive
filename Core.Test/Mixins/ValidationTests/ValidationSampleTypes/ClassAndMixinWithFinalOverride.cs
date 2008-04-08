using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class ClassWithFinalMethod
  {
    public sealed override string ToString()
    {
      return "";
    }
  }

  public class MixinForFinalMethod
  {
    [OverrideTarget]
    public new string ToString ()
    {
      return "";
    }
  }
}
