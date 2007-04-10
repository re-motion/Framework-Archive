using System;
using Rubicon.Utilities;

namespace Mixins
{
  public class Mixin<[This]TThis, [Base]TBase>
      where TThis : class
      where TBase : class
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
    internal void Initialize([This]TThis @this, [Base]TBase @base)
    {
      Assertion.Assert (@this != null);
      Assertion.Assert (@base != null || typeof (INull).IsAssignableFrom (typeof(TBase)));
      _this = @this;
      _base = @base;
    }
  }

  public class Mixin<TThis> : Mixin<TThis, INull>
    where TThis : class
  {
  }

  public interface INull
  {
  }
}
