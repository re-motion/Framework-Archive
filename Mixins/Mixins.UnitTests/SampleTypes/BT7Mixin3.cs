using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT7Mixin3
  {
    string One ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin3 : Mixin<object, IBT7Mixin1>, IBT7Mixin3
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin3.One-" + Base.BT7Mixin1Specific() + "-" + Base.One();
    }
  }
}
