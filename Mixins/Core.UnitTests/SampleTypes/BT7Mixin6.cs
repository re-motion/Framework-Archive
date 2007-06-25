using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IBT7Mixin6 { }

  // no attribute
  public class BT7Mixin6 : Mixin<object, IBaseType7>, IBT7Mixin6
  {
    [Override]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin6.One(" + t + ")-" + Base.One(t);
    }
  }
}
