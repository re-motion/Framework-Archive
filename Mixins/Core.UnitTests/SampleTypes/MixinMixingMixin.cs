using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (MixinMixingClass))]
  public class MixinMixingMixin : Mixin<MixinMixingClass, MixinMixingMixin.IRequirements>
  {
    public interface IRequirements
    {
      string StringMethod (int i);
    }

    [Override]
    public virtual string StringMethod (int i)
    {
      return "MixinMixingMixin-" + Base.StringMethod (i);
    }
  }
}