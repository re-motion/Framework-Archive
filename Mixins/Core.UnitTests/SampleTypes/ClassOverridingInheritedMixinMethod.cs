using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (MixinWithInheritedMethod))]
  public class ClassOverridingInheritedMixinMethod
  {
    [Override]
    public string ProtectedInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod";
    }

    [Override]
    public string ProtectedInternalInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod";
    }

    [Override]
    public string PublicInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.PublicInheritedMethod";
    }
  }

  public class BaseMixinWithInheritedMethod : Mixin<object>
  {
    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInheritedMethod";
    }

    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.PublicInheritedMethod";
    }
  }

  public class MixinWithInheritedMethod : BaseMixinWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod () + "-" + ProtectedInternalInheritedMethod() + "-" + PublicInheritedMethod();
    }
  }
}