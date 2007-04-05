using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT5Mixin3
  {
    void Foo ();
  }

  [MixinFor(typeof(BaseType3))]
  public class BT5Mixin3 : Mixin<IBT5Mixin3, IBT5Mixin3>, IBT5Mixin3
  {
    public void Foo ()
    {
    }
  }
}
