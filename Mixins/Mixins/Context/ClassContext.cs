using System;
using System.Collections.Generic;

namespace Mixins.Context
{
  public class ClassContext
  {
    private Type _type;
    private List<MixinContext> _mixinContexts = new List<MixinContext> ();

    public ClassContext (Type type)
    {
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<MixinContext> MixinContexts
    {
      get { return _mixinContexts; }
    }

    public void AddMixinContext (MixinContext mixinContext)
    {
      if (mixinContext.TargetType != Type)
      {
        string message = string.Format("Cannot add mixin definition for different type {0} to context of class {1}.", mixinContext.TargetType, Type);
        throw new ArgumentException (message, "mixinContext");
      }
      _mixinContexts.Add (mixinContext);
    }
  }
}