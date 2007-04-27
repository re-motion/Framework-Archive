using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon.Utilities;
using System.Reflection;

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
        extensions[i] = InstantiateMixin (enumerator.Current, instance);
      }

      InitializeInstanceFields (instance, extensions);
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition, object mixinTargetInstance)
    {
      Type mixinType = mixinDefinition.Type;
      if (mixinType.ContainsGenericParameters)
      {
        Type[] mixinParameters = Array.FindAll (mixinType.GetGenericArguments(), delegate (Type t) { return t.IsGenericParameter; });
        Type[] assignedTypes =
            Array.ConvertAll<Type, Type> (mixinParameters, delegate (Type parameter) { return FindAssignedType (parameter, mixinTargetInstance); });
        mixinType = mixinType.MakeGenericType (assignedTypes);
      }
      object mixinInstance = Activator.CreateInstance (mixinType);
      InitializeMixin (mixinDefinition, mixinInstance, mixinTargetInstance);
      return mixinInstance;
    }

    private static Type FindAssignedType (Type parameter, object mixinTargetInstance)
    {
      if (parameter.IsDefined (typeof (ThisAttribute), false))
      {
        return mixinTargetInstance.GetType();
      }
      else
      {
        throw new NotImplementedException ("Base parameter types are not implemented.");
      }
    }

    private static void InitializeMixin(MixinDefinition mixinDefinition, object mixinInstance, object mixinTargetInstance)
    {
      foreach (MethodDefinition initializationMethod in mixinDefinition.InitializationMethods)
      {
        ParameterInfo[] methodArguments = initializationMethod.MethodInfo.GetParameters();
        object[] argumentValues = Array.ConvertAll<ParameterInfo, object> (methodArguments,
            delegate (ParameterInfo p) { return GetMixinArgumentInitialization (p, mixinTargetInstance); });
        initializationMethod.MethodInfo.Invoke (mixinInstance, argumentValues);
      }
    }

    private static object GetMixinArgumentInitialization (ParameterInfo p, object mixinTargetInstance)
    {
      if (p.IsDefined (typeof (ThisAttribute), false))
        return mixinTargetInstance;
      else if (p.IsDefined (typeof (BaseAttribute), false))
        throw new NotImplementedException ("Base parameter types are not implemented.");
      else
        throw new NotSupportedException ("Initialization methods can only contain this or base arguments.");
    }
  }
}
