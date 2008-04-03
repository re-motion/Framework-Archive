using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Serializable]
  public class ClassOverridingSpecificMixinMember
  {
    [OverrideMixin (typeof (MixinWithVirtualMethod))]
    public virtual string VirtualMethod ()
    {
      return "ClassOverridingSpecificMixinMember.ToString";
    }
  }
}
