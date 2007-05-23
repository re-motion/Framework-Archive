using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon.Utilities;
using System.Reflection;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;
using Mixins.Utilities;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public static class GeneratedClassInstanceInitializer
  {
    public static void InitializeInstanceFields (object instance)
    {
      InitializeInstanceFieldsWithMixins (instance, null);
    }

    public static void InitializeInstanceFieldsWithMixins (object instance, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      IMixinTarget mixinTarget = instance as IMixinTarget;
      if (mixinTarget == null)
      {
        throw new ArgumentException ("Object is not a mixin target.", "instance");
      }

      BaseClassDefinition configuration = mixinTarget.Configuration;

      InitializeFirstProxy (instance);

      object[] extensions = PrepareExtensionsWithGivenMixinInstances (configuration, mixinInstances);
      FillUpExtensionsWithNewMixinInstances (extensions, configuration, instance);

      SetExtensionsField (instance, extensions);
    }

    private static void InitializeFirstProxy (object instance)
    {
      Type type = instance.GetType ();
      Type baseCallProxyType = type.GetNestedType ("BaseCallProxy");
      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, instance, 0);
      type.GetField ("__first").SetValue (instance, firstBaseCallProxy);
    }

    private static void SetExtensionsField (object instance, object[] extensions)
    {
      Type type = instance.GetType ();
      type.GetField ("__extensions").SetValue (instance, extensions);
    }

    public static void InitializeInstanceFields (object instance, object[] extensions)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("extensions", extensions);

      if (!(instance is IMixinTarget))
      {
        throw new ArgumentException ("Object is not a mixin target.", "instance");
      }

      SetExtensionsField(instance, extensions);
      InitializeFirstProxy (instance);
    }

    private static object[] PrepareExtensionsWithGivenMixinInstances (BaseClassDefinition configuration, object[] mixinInstances)
    {
      object[] extensions = new object[configuration.Mixins.Count];

      if (mixinInstances != null)
      {
        foreach (object mixinInstance in mixinInstances)
        {
          MixinDefinition mixinDefinition = configuration.Mixins[mixinInstance.GetType()];
          if (mixinDefinition == null)
          {
            string message = string.Format ("The supplied mixin of type {0} is not valid in the current configuration.", mixinInstance.GetType());
            throw new ArgumentException (message, "mixinInstances");
          }
          else
            extensions[mixinDefinition.MixinIndex] = mixinInstance;
        }
      }
      return extensions;
    }

    private static void FillUpExtensionsWithNewMixinInstances (
        object[] extensions, BaseClassDefinition configuration, object targetInstance)
    {
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition, targetInstance);
      }
    }

    private static object InstantiateBaseCallProxy (Type baseCallProxyType, object targetInstance, int depth)
    {
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth});
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition, object mixinTargetInstance)
    {
      Type mixinType = mixinDefinition.Type;
      Assertion.Assert (!mixinType.ContainsGenericParameters);

      if (mixinDefinition.HasOverriddenMembers())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);

      object mixinInstance = Activator.CreateInstance (mixinType);

      InitializeMixinInstance (mixinDefinition, mixinInstance, mixinTargetInstance);
      return mixinInstance;
    }

    public static void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, object mixinTargetInstance)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      ArgumentUtility.CheckNotNull ("mixinInstance", mixinInstance);
      ArgumentUtility.CheckNotNull ("mixinTargetInstance", mixinTargetInstance);

      Type baseCallProxyType = MixinReflector.GetBaseCallProxyType (mixinTargetInstance);
      object baseCallProxyInstance = InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex + 1);
      InvokeMixinInitializationMethod (mixinDefinition, mixinInstance, mixinTargetInstance, baseCallProxyInstance);
    }

    private static void InvokeMixinInitializationMethod (MixinDefinition mixinDefinition, object mixinInstance, object mixinTargetInstance,
        object baseCallProxyInstance)
    {
      MethodInfo initializationMethod = MixinReflector.GetInitializationMethod (mixinInstance.GetType ());
      if (initializationMethod != null)
      {
        Assertion.Assert (!initializationMethod.ContainsGenericParameters);

        ParameterInfo[] methodArguments = initializationMethod.GetParameters ();
        object[] argumentValues = new object[methodArguments.Length];
        for (int i = 0; i < argumentValues.Length; ++i)
          argumentValues[i] = GetMixinInitializationArgument (methodArguments[i], mixinTargetInstance, baseCallProxyInstance, mixinDefinition);

        try
        {
          initializationMethod.Invoke (mixinInstance, argumentValues); // TODO: perhaps cache this
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    private static object GetMixinInitializationArgument (ParameterInfo parameter, object mixinTargetInstance, object baseCallProxyInstance,
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
