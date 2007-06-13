using System;

namespace Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType7))]
  public class BT7Mixin0 : Mixin<object, IBT7Mixin2>
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin0.One-" + Base.One();
    }
  }
}
