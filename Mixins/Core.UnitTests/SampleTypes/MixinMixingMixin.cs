using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (MixinMixingClass))]
  public class MixinMixingMixin : Mixin<MixinMixingClass, MixinMixingMixin.IRequirements>
  {
    public interface IRequirements
    {
      string StringMethod (int i);
    }

    [OverrideTarget]
    public virtual string StringMethod (int i)
    {
      return "MixinMixingMixin-" + Base.StringMethod (i);
    }
  }
}