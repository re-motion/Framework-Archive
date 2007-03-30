using System;

namespace Mixins
{
  public class Mixin<TThis, TBase>
  {
    private TThis _this;
    private TBase _base;

    protected TThis This
    {
      get { return _this; }
    }

    protected TBase Base
    {
      get { return _base; }
    }

    [MixinInitializationMethod]
    internal void Initialize(TThis @this, TBase @base)
    {
      _this = @this;
      _base = @base;
    }
  }

  public class Mixin<TThis> : Mixin<TThis, INull>
  {
  }

  public interface INull
  {
  }
}
