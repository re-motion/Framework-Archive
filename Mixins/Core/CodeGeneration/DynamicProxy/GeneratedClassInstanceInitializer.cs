using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;
using System.Reflection;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;
using Rubicon.Mixins.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  public static class GeneratedClassInstanceInitializer
  {
    public static void InitializeMixinTarget (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      object[] mixinInstances = MixedTypeInstantiationScope.Current.SuppliedMixinInstances;
      BaseClassDefinition configuration = mixinTarget.Configuration;

      InitializeFirstProxy (mixinTarget);

      object[] extensions = PrepareExtensionsWithGivenMixinInstances (configuration, mixinInstances);
      FillUpExtensionsWithNewMixinInstances (extensions, configuration, mixinTarget);

      SetExtensionsField (mixinTarget, extensions);
      InitializeMixinInstances (extensions, configuration, mixinTarget);
    }

    public static void InitializeDeserializedMixinTarget (IMixinTarget mixinTarget, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstances);

      BaseClassDefinition configuration = mixinTarget.Configuration;

      InitializeFirstProxy (mixinTarget);
      SetExtensionsField (mixinTarget, mixinInstances);
      InitializeMixinInstances (mixinInstances, configuration, mixinTarget);
    }


    private static void InitializeFirstProxy (IMixinTarget mixinTarget)
    {
      Type type = mixinTarget.GetType ();
      Type baseCallProxyType = type.GetNestedType ("BaseCallProxy");
      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, mixinTarget, 0);
      type.GetField ("__first").SetValue (mixinTarget, firstBaseCallProxy);
    }

    private static object InstantiateBaseCallProxy (Type baseCallProxyType, IMixinTarget targetInstance, int depth)
    {
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth });
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
        object[] extensions, BaseClassDefinition configuration, IMixinTarget targetInstance)
    {
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition, targetInstance);
      }
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition, IMixinTarget mixinTargetInstance)
    {
      Type mixinType = mixinDefinition.Type;
      Assertion.Assert (!mixinType.ContainsGenericParameters);

      if (mixinDefinition.HasOverriddenMembers())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);

      object mixinInstance;
      try
      {
        mixinInstance = Activator.CreateInstance (mixinType);
      }
      catch (MissingMethodException ex)
      {
        string message = string.Format ("Cannot instantiate mixin {0}, there is no public default constructor.",
            mixinDefinition.Type);
        throw new MissingMethodException (message, ex);
      }

      return mixinInstance;
    }

    private static void SetExtensionsField (IMixinTarget mixinTarget, object[] extensions)
    {
      Type type = mixinTarget.GetType ();
      type.GetField ("__extensions").SetValue (mixinTarget, extensions);
    }

    private static void InitializeMixinInstances (object[] mixins, BaseClassDefinition configuration, IMixinTarget mixinTargetInstance)
    {
      Assertion.Assert (mixins.Length == configuration.Mixins.Count);
      for (int i = 0; i < mixins.Length; ++i)
        InitializeMixinInstance (configuration.Mixins[i], mixins[i], mixinTargetInstance);
    }

    private static void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget mixinTargetInstance)
    {
      Type baseCallProxyType = MixinReflector.GetBaseCallProxyType (mixinTargetInstance);
      object baseCallProxyInstance = InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex + 1);
      InvokeMixinInitializationMethod (mixinDefinition, mixinInstance, mixinTargetInstance, baseCallProxyInstance);
    }

    private static void InvokeMixinInitializationMethod (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget mixinTargetInstance,
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
          initializationMethod.Invoke (mixinInstance, argumentValues);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
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
