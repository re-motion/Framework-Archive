using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class ClassOverridingMixinMethodWrongSig
  {
    [OverrideMixin]
    public string AbstractMethod(string s)
    {
      return "ClassOverridingMixinMethod.AbstractMethod" + s;
    }
  }
}
