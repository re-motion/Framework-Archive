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
      Type baseCallProxyType = FindBaseCallProxyType (type);
      Assertion.IsNotNull (baseCallProxyType);

      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, mixinTarget, 0);
      type.GetField ("__first").SetValue (mixinTarget, firstBaseCallProxy);
    }

    private static Type FindBaseCallProxyType (Type type)
    {
      Assertion.IsNotNull (type);
      Type baseCallProxyType;
      do
      {
        baseCallProxyType = type.GetNestedType ("BaseCallProxy");
        type = type.BaseType;
      } while (baseCallProxyType == null && type != null);
      return baseCallProxyType;
    }

    private static object InstantiateBaseCallProxy (Type baseCallProxyType, IMixinTarget targetInstance, int depth)
    {
      Assertion.IsNotNull (baseCallProxyType);
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth });
    }

    private static object[] PrepareExtensionsWithGivenMixinInstances (BaseClassDefinition configuration, object[] mixinInstances)
    {
      object[] extensions = new object[configuration.Mixins.Count];

      if (mixinInstances != null)
      {
        foreach (object mixinInstance in mixinInstances)
        {
          ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstance);
          MixinDefinition mixinDefinition = GetMixinDefinitionFromMixinInstance(mixinInstance, configuration);
          if (mixinDefinition == null)
          {
            string message = string.Format ("The supplied mixin of type {0} is not valid in the current configuration.", mixinInstance.GetType());
            throw new ArgumentException (message, "mixinInstances");
          }
          else if (TypeGenerator.NeedsDerivedMixinType (mixinDefinition) && !ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).IsInstanceOfType (mixinInstance))
          {
            string message = string.Format ("The mixin {0} applied to base type {1} needs to have a subclass generated at runtime. It is therefore "
                + "not possible to use the given object of type {2} as a mixin instance.", mixinDefinition.FullName, configuration.FullName,
                mixinInstance.GetType().Name);
            throw new ArgumentException (message, "mixinInstances");
          }
          else
            extensions[mixinDefinition.MixinIndex] = mixinInstance;
        }
      }
      return extensions;
    }

    private static MixinDefinition GetMixinDefinitionFromMixinInstance (object mixinInstance, BaseClassDefinition baseClassDefinition)
    {
      Type mixinType = mixinInstance.GetType();
      ConcreteMixinTypeAttribute[] mixinTypeAttributes =
          (ConcreteMixinTypeAttribute[]) mixinType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
          
      if (mixinTypeAttributes.Length > 0)
      {
        Assertion.IsTrue (mixinTypeAttributes.Length == 1, "AllowMultiple == false");
        return mixinTypeAttributes[0].GetMixinDefinition();
      }
      else
        return baseClassDefinition.Mixins[mixinType];
    }

    private static void FillUpExtensionsWithNewMixinInstances (
        object[] extensions, BaseClassDefinition configuration, IMixinTarget targetInstance)
    {
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition);
      }
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition)
    {
      Type mixinType = mixinDefinition.Type;
      Assertion.IsFalse (mixinType.ContainsGenericParameters);

      if (TypeGenerator.NeedsDerivedMixinType (mixinDefinition))
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
      Assertion.IsTrue (mixins.Length == configuration.Mixins.Count);
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
        Assertion.IsFalse (initializationMethod.ContainsGenericParameters);

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
