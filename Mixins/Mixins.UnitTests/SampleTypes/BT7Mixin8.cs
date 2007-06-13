using System;

namespace Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType7))]
  public class BT7Mixin8 : Mixin<object, IBaseType7>
  {
    [Override]
    public string Five()
    {
      return "BT7Mixin8.Five-" + Base.Five();
    }
  }
}
