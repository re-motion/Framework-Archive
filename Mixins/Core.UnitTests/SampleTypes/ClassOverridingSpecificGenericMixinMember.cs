using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Serializable]
  public class ClassOverridingSpecificGenericMixinMember
  {
    [OverrideMixin (typeof (GenericMixinWithVirtualMethod<>))]
    public string VirtualMethod ()
    {
      return "ClassOverridingSpecificGenericMixinMember.ToString";
    }
  }
}
