using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType7))]
  public class BT7Mixin0 : Mixin<object, IBT7Mixin2>
  {
    [Override]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin0.One(" + t + ")-" + Base.One(t);
    }
  }
}
