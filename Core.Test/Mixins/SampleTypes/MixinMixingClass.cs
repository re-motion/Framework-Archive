using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (ClassWithMixedMixin))]
  public class MixinMixingClass : Mixin<ClassWithMixedMixin, MixinMixingClass.IRequirements>
  {
    public interface IRequirements
    {
      string StringMethod (int i);
    }

    [OverrideTarget]
    public virtual string StringMethod (int i)
    {
      return "MixinMixingClass-" + Base.StringMethod (i);
    }
  }
}