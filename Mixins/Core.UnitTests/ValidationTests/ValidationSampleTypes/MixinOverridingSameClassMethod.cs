using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  public class MixinOverridingSameClassMethod
  {
    [OverrideTarget]
    public virtual string AbstractMethod(int i)
    {
      return "MixinOverridingSameClassMethod.AbstractMethod-" + i;
    }
  }
}
