using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT3Mixin4
  {
    void Foo ();
  }

  [MixinFor (typeof (BaseType3))]
  public class BT3Mixin4 : BT3Mixin3<BaseType3, IBaseType34>, IBT3Mixin4
  {
    public void Foo ()
    {
    }
  }
}
