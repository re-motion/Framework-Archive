using System;
using System.Diagnostics;
using Rubicon.Utilities;
using Mixins.Utilities;

namespace Mixins
{
  public static class Mixin
  {
    public static TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get<TMixin> (mixinTarget);
    }

    public static object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get (mixinType, mixinTarget);
    }
  }

  [Serializable]
  public class Mixin<[This]TThis, [Base]TBase>
      where TThis: class
      where TBase: class
  {
    private TThis _this;
    private TBase _base;

    protected TThis This
    {
      [DebuggerStepThrough]
      get
      {
        if (_this == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _this;
      }
    }

    protected TBase Base
    {
      [DebuggerStepThrough]
      get
      {
        if (_base == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _base;
      }
    }

    internal void Initialize ([This] TThis @this, [Base] TBase @base)
    {
      Assertion.Assert (@this != null);
      Assertion.Assert (@base != null);
      _this = @this;
      _base = @base;
      OnInitialized();
    }

    protected virtual void OnInitialized()
    {
      // nothing
    }
  }

  [Serializable]
  public class Mixin<[This]TThis>
      where TThis: class
  {
    private TThis _this;

    protected TThis This
    {
      [DebuggerStepThrough]
      get
      {
        if (_this == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _this;
      }
    }

    internal void Initialize ([This] TThis @this)
    {
      Assertion.Assert (@this != null);
      _this = @this;
      OnInitialized();
    }

    protected virtual void OnInitialized()
    {
      // nothing
    }
  }
}
