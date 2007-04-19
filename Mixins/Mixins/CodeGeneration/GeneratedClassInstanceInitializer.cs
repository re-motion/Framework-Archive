using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public static class GeneratedClassInstanceInitializer
  {
    public static void InitializeInstanceFields (object instance, object[] extensions)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("instance", extensions);

      Type type = instance.GetType ();
      type.GetField ("__extensions").SetValue (instance, extensions);
    }

    public static void InitializeInstanceFields (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      IMixinTarget mixinTarget = instance as IMixinTarget;
      if (mixinTarget == null)
      {
        throw new ArgumentException ("Object is not a mixin target.", "instance");
      }

      BaseClassDefinition configuration = ((IMixinTarget) instance).Configuration;
      object[] extensions = new object[configuration.Mixins.Count];
      IEnumerator<MixinDefinition> enumerator = configuration.Mixins.GetEnumerator ();
      for (int i = 0; enumerator.MoveNext (); ++i)
      {
        extensions[i] = InstantiateMixin (enumerator.Current);
      }

      InitializeInstanceFields (instance, extensions);
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition)
    {
      object instance = Activator.CreateInstance (mixinDefinition.Type);
      // TODO: call initialization methods
      return instance;
    }
  }
}
