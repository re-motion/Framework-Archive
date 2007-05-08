using System;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins
{
  public static class Mixin
  {
    public static TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      return (TMixin) Get (typeof (TMixin), mixinTarget);
    }

    public static object Get (Type mixinType, object mixinTarget)
    {
      IMixinTarget castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        MixinDefinition mixinDefinition = castMixinTarget.Configuration.Mixins[mixinType];
        if (mixinDefinition != null)
        {
          return castMixinTarget.Mixins[mixinDefinition.MixinIndex];
        }
      }
      return null;
    }
  }

  public class Mixin<[This]TThis, [Base]TBase>
      where TThis : class
      where TBase : class
  {
    private TThis _this;
    private TBase _base;

    protected TThis This
    {
      get
      {
        if (_this == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _this;
      }
    }

    protected TBase Base
    {
      get
      {
        if (_base == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _base;
      }
    }

    [MixinInitializationMethod]
    internal void Initialize([This]TThis @this, [Base]TBase @base)
    {
      Assertion.Assert (@this != null);
      Assertion.Assert (@base != null);
      _this = @this;
      _base = @base;
      OnInitialized ();
    }

    protected virtual void OnInitialized ()
    {
      // nothing
    }
  }

  public class Mixin<[This]TThis>
    where TThis : class
  {
    private TThis _this;

    protected TThis This
    {
      get
      {
        if (_this == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _this;
      }
    }

    [MixinInitializationMethod]
    internal void Initialize ([This]TThis @this)
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
