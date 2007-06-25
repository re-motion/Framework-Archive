using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
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
    [Override]
    public new string ToString ()
    {
      return "";
    }
  }
}
