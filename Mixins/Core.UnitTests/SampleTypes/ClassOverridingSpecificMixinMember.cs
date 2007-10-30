using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Serializable]
  public class ClassOverridingSpecificMixinMember
  {
    [OverrideMixin (typeof (MixinWithVirtualMethod))]
    public string VirtualMethod ()
    {
      return "ClassOverridingSpecificMixinMember.ToString";
    }
  }
}
