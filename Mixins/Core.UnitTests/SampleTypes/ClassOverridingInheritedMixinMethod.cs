using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (MixinWithInheritedMethod))]
  public class ClassOverridingInheritedMixinMethod
  {
    [Override]
    public string InheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.InheritedMethod";
    }
  }

  public class BaseMixinWithInheritedMethod : Mixin<object>
  {
    public virtual string InheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.InheritedMethod";
    }
  }

  public class MixinWithInheritedMethod : BaseMixinWithInheritedMethod
  {
  }
}