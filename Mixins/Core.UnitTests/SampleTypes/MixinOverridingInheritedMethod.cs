using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinOverridingInheritedMethod : Mixin<object, MixinOverridingInheritedMethod.IBaseMethods>
  {
    public interface IBaseMethods
    {
      string InheritedMethod ();
    }

    [Override]
    public string InheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.InheritedMethod-" + Base.InheritedMethod ();
    }
  }

  public class BaseClassWithInheritedMethod
  {
    public virtual string InheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.InheritedMethod";
    }
  }

  [Uses (typeof (MixinOverridingInheritedMethod))]
  public class ClassWithInheritedMethod : BaseClassWithInheritedMethod
  {
  }
}