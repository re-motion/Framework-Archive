using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Mixins.Utilities;
using Mixins.CodeGeneration.DynamicProxy;
using Mixins.Definitions;

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
  public class Mixin<[This]TThis, [Base]TBase> : Mixin<TThis>
      where TThis: class
      where TBase: class
  {
    [NonSerialized]
    private TBase _base;

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

    internal void Initialize ([This] TThis @this, [Base] TBase @base, [Configuration] MixinDefinition configuration)
    {
      Assertion.Assert (@this != null);
      Assertion.Assert (@base != null);
      _base = @base;
      base.Initialize (@this, configuration);
    }
  }

  [Serializable]
  public class Mixin<[This]TThis>
      where TThis: class
  {
    private TThis _this;
    private MixinDefinition _configuration;

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

    protected MixinDefinition Configuration
    {
      [DebuggerStepThrough]
      get
      {
        if (_configuration == null)
          throw new InvalidOperationException ("Mixin has not yet been initialized.");
        return _configuration;
      }
    }

    internal void Initialize ([This] TThis @this, [Configuration] MixinDefinition configuration)
    {
      Assertion.Assert (@this != null);
      _this = @this;
      _configuration = @configuration;
      OnInitialized();
    }

    protected virtual void OnInitialized()
    {
      // nothing
    }
  }
}
