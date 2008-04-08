using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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
