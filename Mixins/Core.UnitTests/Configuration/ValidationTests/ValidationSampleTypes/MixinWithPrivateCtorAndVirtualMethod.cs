using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
{
  public class MixinWithPrivateCtorAndVirtualMethod : Mixin<object>
  {
    public static MixinWithPrivateCtorAndVirtualMethod Create ()
    {
      return new MixinWithPrivateCtorAndVirtualMethod ();
    }

    private MixinWithPrivateCtorAndVirtualMethod ()
    {
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinWithPrivateCtorAndVirtualMethod.OverriddenMethod(" + i + ")";
    }
  }
}
