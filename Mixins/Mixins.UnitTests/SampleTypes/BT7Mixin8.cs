using System;

namespace Mixins.UnitTests.SampleTypes
{
  // no attributes
  public class BT7Mixin8 : Mixin<object, IBaseType7>
  {
    [Override]
    public string Five()
    {
      return "BT7Mixin8.Five-" + Base.Five();
    }
  }
}
