using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinOverridingInheritedMethod : Mixin<object, MixinOverridingInheritedMethod.IBaseMethods>
  {
    public interface IBaseMethods
    {
      string ProtectedInheritedMethod ();
      string ProtectedInternalInheritedMethod ();
      string PublicInheritedMethod ();
    }

    [Override]
    public string ProtectedInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInheritedMethod-" + Base.ProtectedInheritedMethod ();
    }

    [Override]
    public string ProtectedInternalInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-" + Base.ProtectedInternalInheritedMethod ();
    }

    [Override]
    public string PublicInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.PublicInheritedMethod-" + Base.PublicInheritedMethod ();
    }
  }

  public class BaseClassWithInheritedMethod
  {
    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.PublicInheritedMethod";
    }
  }

  [Uses (typeof (MixinOverridingInheritedMethod))]
  public class ClassWithInheritedMethod : BaseClassWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod ()+ "-" + ProtectedInternalInheritedMethod() + "-" +  PublicInheritedMethod();
    }
  }
}