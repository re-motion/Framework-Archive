using System;
using System.Collections.Generic;

namespace Mixins.Context
{
  public class ClassContext
  {
    private Type _type;
    private List<MixinDefinition> _mixinDefinitions = new List<MixinDefinition> ();

    public ClassContext (Type type)
    {
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<MixinDefinition> MixinDefinitions
    {
      get { return _mixinDefinitions; }
    }

    public void AddMixinDefinition (MixinDefinition mixinDefinition)
    {
      if (mixinDefinition.TargetType != Type)
      {
        string message = string.Format("Cannot add mixin definition for different type {0} to context of class {1}.", mixinDefinition.TargetType, Type);
        throw new ArgumentException (message, "mixinDefinition");
      }
      _mixinDefinitions.Add (mixinDefinition);
    }
  }
}