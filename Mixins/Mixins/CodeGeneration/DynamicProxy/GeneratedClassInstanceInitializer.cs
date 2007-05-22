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
      Type baseCallProxyType = instance.GetType ().GetNestedType ("BaseCallProxy");

      object[] extensions = PrepareExtensionsWithGivenMixinInstances (configuration, mixinInstances);
      FillUpExtensionsWithNewMixinInstances (extensions, configuration, instance, baseCallProxyType);

      InitializeInstanceFields (instance, extensions);
    }


    public static void InitializeInstanceFields (object instance, object[] extensions)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("extensions", extensions);

      Type type = instance.GetType ();
      type.GetField ("__extensions").SetValue (instance, extensions);

      Type baseCallProxyType = instance.GetType ().GetNestedType ("BaseCallProxy");
      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, instance, 0);
      type.GetField ("__first").SetValue (instance, firstBaseCallProxy);
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
        object[] extensions, BaseClassDefinition configuration, object targetInstance, Type baseCallProxyType)
    {
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition, targetInstance, baseCallProxyType);
      }
    }

    private static object InstantiateBaseCallProxy (Type baseCallProxyType, object targetInstance, int depth)
    {
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth});
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition, object mixinTargetInstance, Type baseCallProxyType)
    {
      object baseCallProxyInstance = InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex + 1);

      Type mixinType = mixinDefinition.Type;
      List<Type> boundGenericParameters = BindGenericParameters(mixinType, mixinTargetInstance, baseCallProxyInstance);

      if (mixinDefinition.HasOverriddenMembers())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition, boundGenericParameters.ToArray());
      else if (mixinType.ContainsGenericParameters)
        mixinType = mixinType.MakeGenericType (boundGenericParameters.ToArray());

      object mixinInstance = Activator.CreateInstance (mixinType);
      InitializeMixin (mixinInstance, mixinTargetInstance, baseCallProxyInstance);
      return mixinInstance;
    }

    private static List<Type> BindGenericParameters(Type mixinType, object mixinTargetInstance, object baseCallProxyInstance)
    {
      List<Type> boundGenericParameters = new List<Type> ();
      if (mixinType.ContainsGenericParameters)
      {
        foreach (Type genericArgument in mixinType.GetGenericArguments())
        {
          if (genericArgument.IsGenericParameter)
            boundGenericParameters.Add (BindGenericParameter (genericArgument, mixinTargetInstance, baseCallProxyInstance));
        }
      }
      return boundGenericParameters;
    }

    private static Type BindGenericParameter (Type parameter, object mixinTargetInstance, object baseCallProxyInstance)
    {
      if (ReflectionUtility.IsGenericParameterAssociatedWithAttribute (parameter, typeof (ThisAttribute)))
        return mixinTargetInstance.GetType().BaseType;
      else if (ReflectionUtility.IsGenericParameterAssociatedWithAttribute (parameter, typeof (BaseAttribute)))
        return baseCallProxyInstance.GetType();
      else
      {
        string message = string.Format ("Generic argument {0} of mixin {1} cannot be bound to a type - it is not marked as This or Base argument.",
            parameter.Name,
            parameter.DeclaringType.FullName);
        throw new NotSupportedException (message);
      }
    }

    private static void InitializeMixin (object mixinInstance, object mixinTargetInstance, object baseCallProxyInstance)
    {
      MethodInfo initializationMethod = MixinReflector.GetInitializationMethod (mixinInstance.GetType ());
      if (initializationMethod != null)
      {
        Assertion.Assert (!initializationMethod.ContainsGenericParameters);

        ParameterInfo[] methodArguments = initializationMethod.GetParameters ();
        object[] argumentValues = new object[methodArguments.Length];
        for (int i = 0; i < argumentValues.Length; ++i)
          argumentValues[i] = GetMixinInitializationArgument (methodArguments[i], mixinTargetInstance, baseCallProxyInstance);

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

    private static object GetMixinInitializationArgument (ParameterInfo p, object mixinTargetInstance, object baseCallProxyInstance)
    {
      if (p.IsDefined (typeof (ThisAttribute), false))
        return mixinTargetInstance;
      else if (p.IsDefined (typeof (BaseAttribute), false))
        return baseCallProxyInstance;
      else
        throw new NotSupportedException ("Initialization methods can only contain this or base arguments.");
    }
  }
}
