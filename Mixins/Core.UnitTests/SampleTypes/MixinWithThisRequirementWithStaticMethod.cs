using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class MixinWithThisRequirementWithStaticMethod : Mixin<ClassWithStaticMethod>
  {
  }

  [Uses (typeof (MixinWithThisRequirementWithStaticMethod))]
  public class ClassWithStaticMethod
  {
    public static void StaticMethod ()
    {
    }

    public void InstanceMethod ()
    {
    }

    public void VirtualMethod ()
    {
    }
  }
}
