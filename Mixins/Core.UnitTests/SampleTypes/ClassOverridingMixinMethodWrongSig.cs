using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class ClassOverridingMixinMethodWrongSig
  {
    [OverrideMixinMember]
    public string AbstractMethod(string s)
    {
      return "ClassOverridingMixinMethod.AbstractMethod" + s;
    }
  }
}
