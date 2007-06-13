using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT7Mixin1
  {
    string One ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin1 : Mixin<object, IBaseType7> , IBT7Mixin1
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin1.One-" + Base.One();
    }
  }
}
