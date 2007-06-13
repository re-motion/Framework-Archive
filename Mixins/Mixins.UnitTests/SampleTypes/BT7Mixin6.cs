using System;

namespace Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType7))]
  public class BT7Mixin6 : Mixin<object, IBaseType7>
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin6.One-" + Base.One();
    }
  }
}
