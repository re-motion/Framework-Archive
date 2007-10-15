using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (ClassWithMixedMixin))]
  public class MixinMixingClass : Mixin<ClassWithMixedMixin, MixinMixingClass.IRequirements>
  {
    public interface IRequirements
    {
      string StringMethod (int i);
    }

    [Override]
    public virtual string StringMethod (int i)
    {
      return "MixinMixingClass-" + Base.StringMethod (i);
    }
  }
}