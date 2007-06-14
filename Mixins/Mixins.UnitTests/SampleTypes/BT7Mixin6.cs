using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT7Mixin6 { }

  // no attribute
  public class BT7Mixin6 : Mixin<object, IBaseType7>, IBT7Mixin6
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin6.One-" + Base.One();
    }
  }
}
