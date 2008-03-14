using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
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
