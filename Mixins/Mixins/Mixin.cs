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
      Assertion.Assert (@base != null);
      _this = @this;
      _base = @base;
    }
  }

  public class Mixin<[This]TThis>
    where TThis : class
  {
    private TThis _this;

    protected TThis This
    {
      get { return _this; }
    }

    [MixinInitializationMethod]
    internal void Initialize ([This]TThis @this)
    {
      Assertion.Assert (@this != null);
      _this = @this;
    }
  }
}
