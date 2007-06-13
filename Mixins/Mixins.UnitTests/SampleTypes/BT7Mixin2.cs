using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT7Mixin2Reqs : IBT7Mixin3, IBaseType7
  {
  }

  public interface IBT7Mixin2
  {
    string One ();
    string Two ();
    string Three ();
    string Four ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin2 : Mixin<BaseType7, IBT7Mixin2Reqs>, IBT7Mixin2
  {
    [Override]
    public virtual string One ()
    {
      return "BT7Mixin2.One-" + ((IBaseType7) Base).One () + "-" + ((IBT7Mixin3) Base).One () + "-" + Base.Two() + "-" + This.Two();
    }

    [Override]
    public virtual string Two()
    {
      return "BT7Mixin2.Two";
    }

    [Override]
    public virtual string Three ()
    {
      return "BT7Mixin2.Three-" + Base.Three ();
    }

    [Override]
    public virtual string Four ()
    {
      return "BT7Mixin2.Four-" + Base.Four();
    }
  }
}
