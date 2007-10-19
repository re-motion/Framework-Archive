using System;
using System.Reflection;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal static class MixinInstanceInitializer
  {
    public static void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget mixinTargetInstance,
        MixinReflector.InitializationMode mode)
    {
      Type baseCallProxyType = MixinReflector.GetBaseCallProxyType (mixinTargetInstance);
      object baseCallProxyInstance = BaseCallProxyInitializer.InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex + 1);
      InvokeMixinInitializationMethod (mixinDefinition, mixinInstance, mixinTargetInstance, baseCallProxyInstance, mode);
    }

    private static void InvokeMixinInitializationMethod (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget mixinTargetInstance,
        object baseCallProxyInstance, MixinReflector.InitializationMode mode)
    {
      MethodInfo initializationMethod = MixinReflector.GetInitializationMethod (mixinInstance.GetType (), mode);
      if (initializationMethod != null)
      {
        Assertion.IsFalse (initializationMethod.ContainsGenericParameters);

        ParameterInfo[] methodArguments = initializationMethod.GetParameters ();
        object[] argumentValues = new object[methodArguments.Length];
        for (int i = 0; i < argumentValues.Length; ++i)
          argumentValues[i] = GetMixinInitializationArgument (methodArguments[i], mixinTargetInstance, baseCallProxyInstance, mixinDefinition);

        initializationMethod.Invoke (mixinInstance, argumentValues);
      }
    }

    private static object GetMixinInitializationArgument (ParameterInfo parameter, IMixinTarget mixinTargetInstance, object baseCallProxyInstance,
        MixinDefinition mixinDefinition)
    {
      if (parameter.IsDefined (typeof (ThisAttribute), false))
        return mixinTargetInstance;
      else if (parameter.IsDefined (typeof (BaseAttribute), false))
        return baseCallProxyInstance;
      else if (parameter.IsDefined (typeof (ConfigurationAttribute), false))
        return mixinDefinition;
      else
        throw new NotSupportedException ("Initialization methods can only contain this or base arguments.");
    }
  }
}
