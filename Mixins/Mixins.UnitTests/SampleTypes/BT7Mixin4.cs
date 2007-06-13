using System;

namespace Mixins.UnitTests.SampleTypes
{
  // no attribute
  public class BT7Mixin4 : Mixin<object, IBaseType7>
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin4.One-" + Base.One();
    }
  }
}
