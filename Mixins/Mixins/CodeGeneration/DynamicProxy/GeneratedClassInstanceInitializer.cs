using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon.Utilities;
using System.Reflection;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public static class GeneratedClassInstanceInitializer
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public static void InitializeInstanceFields (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      IMixinTarget mixinTarget = instance as IMixinTarget;
      if (mixinTarget == null)
      {
        throw new ArgumentException ("Object is not a mixin target.", "instance");
      }

      Type baseCallProxyType = instance.GetType().GetNestedType ("BaseCallProxy");

      BaseClassDefinition configuration = ((IMixinTarget) instance).Configuration;
      object[] extensions = new object[configuration.Mixins.Count];
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
        extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition, instance, baseCallProxyType);

      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, instance, configuration.Mixins.Count);
      InitializeInstanceFields (instance, extensions, firstBaseCallProxy);
    }

    public static void InitializeInstanceFields (object instance, object[] extensions, object firstBaseCallProxy)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("instance", extensions);
      ArgumentUtility.CheckNotNull ("firstBaseCallProxy", firstBaseCallProxy);

      Type type = instance.GetType ();
      type.GetField ("__extensions").SetValue (instance, extensions);
      type.GetField ("__first").SetValue (instance, firstBaseCallProxy);
    }

    private static object InstantiateBaseCallProxy (Type baseCallProxyType, object targetInstance, int depth)
    {
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth});
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition, object mixinTargetInstance, Type baseCallProxyType)
    {
      object baseCallProxyInstance = InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex);

      Type mixinType = mixinDefinition.Type;
      List<Type> boundGenericArguments = new List<Type> ();
      if (mixinType.ContainsGenericParameters)
      {
        foreach (Type genericArgument in mixinType.GetGenericArguments())
        {
          if (genericArgument.IsGenericParameter)
            boundGenericArguments.Add (GetBoundGenericParameter (genericArgument, mixinTargetInstance, baseCallProxyInstance));
        }
      }

      if (mixinDefinition.HasOverriddenMembers())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition, boundGenericArguments.ToArray());
      else if (mixinType.ContainsGenericParameters)
        mixinType = mixinType.MakeGenericType (boundGenericArguments.ToArray());

      object mixinInstance = Activator.CreateInstance (mixinType);
      InitializeMixin (mixinDefinition, mixinInstance, mixinTargetInstance, baseCallProxyInstance);
      return mixinInstance;
    }

    private static Type GetBoundGenericParameter (Type parameter, object mixinTargetInstance, object baseCallProxyInstance)
    {
      if (IsGenericParameterBoundTo (parameter, typeof (ThisAttribute)))
      {
        return mixinTargetInstance.GetType().BaseType;
      }
      else if (IsGenericParameterBoundTo (parameter, typeof (BaseAttribute)))
      {
        return baseCallProxyInstance.GetType();
      }
      else
      {
        string message = string.Format (
            "Generic argument {0} of mixin {1} cannot be bound to a type - it is not marked as This or Base argument.",
            parameter.Name,
            parameter.DeclaringType.FullName);
        throw new NotSupportedException (message);
      }
    }

    private static bool IsGenericParameterBoundTo (Type genericParameter, Type attributeType)
    {
      Type mixinType = genericParameter.DeclaringType;
      Type baseClass = mixinType.BaseType;
      if (!baseClass.IsGenericType)
        return false;

      Type baseClassDefinition = baseClass.GetGenericTypeDefinition();
      Type[] baseClassGenericParameters = baseClassDefinition.GetGenericArguments();

      foreach (Type baseGenericArgument in baseClass.GetGenericArguments())
      {
        if (baseGenericArgument.Equals (genericParameter))
        {
          return baseClassGenericParameters[baseGenericArgument.GenericParameterPosition].IsDefined (attributeType, false);
        }
      }
      return false;
    }

    private static void InitializeMixin (
        MixinDefinition mixinDefinition, object mixinInstance, object mixinTargetInstance, object baseCallProxyInstance)
    {
      foreach (MethodDefinition initializationMethod in mixinDefinition.InitializationMethods)
      {
        ParameterInfo[] methodArguments = initializationMethod.MethodInfo.GetParameters();
        object[] argumentValues = Array.ConvertAll<ParameterInfo, object> (
            methodArguments,
            delegate (ParameterInfo p) { return GetMixinArgumentInitialization (p, mixinTargetInstance, baseCallProxyInstance); });

        MethodInfo concreteInitializationMethod = GetConcreteMethod (mixinInstance, initializationMethod);
        concreteInitializationMethod.Invoke (mixinInstance, argumentValues);
      }
    }

    private static MethodInfo GetConcreteMethod (object instance, MethodDefinition method)
    {
      if (method.MethodInfo.ContainsGenericParameters)
      {
        Assertion.Assert (!method.MethodInfo.IsGenericMethodDefinition);
        return ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (method.MethodInfo, instance.GetType());
      }
      else
        return method.MethodInfo;
    }

    private static object GetMixinArgumentInitialization (ParameterInfo p, object mixinTargetInstance, object baseCallProxyInstance)
    {
      if (p.IsDefined (typeof (ThisAttribute), false))
        return mixinTargetInstance;
      else if (p.IsDefined (typeof (BaseAttribute), false))
        return baseCallProxyInstance;
      else
        throw new NotSupportedException ("Initialization methods can only contain this or base arguments.");
    }

    public static void InitializeInstanceFieldsWithMixins (object instance, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      IMixinTarget mixinTarget = instance as IMixinTarget;
      if (mixinTarget == null)
      {
        throw new ArgumentException ("Object is not a mixin target.", "instance");
      }

      Type baseCallProxyType = instance.GetType ().GetNestedType ("BaseCallProxy");

      BaseClassDefinition configuration = ((IMixinTarget) instance).Configuration;
      object[] extensions = new object[configuration.Mixins.Count];

      foreach (object mixinInstance in mixinInstances)
      {
        MixinDefinition mixinDefinition = configuration.Mixins[mixinInstance.GetType()];
        if (mixinDefinition == null)
        {
          string message = string.Format ("The supplied mixin of type {0} is not valid in the current configuration.", mixinInstance.GetType());
          throw new ArgumentException (message, "mixinInstances");
        }
        else
        {
          extensions[mixinDefinition.MixinIndex] = mixinInstance;
        }
      }

      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition, instance, baseCallProxyType);
      }

      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, instance, configuration.Mixins.Count);
        InitializeInstanceFields (instance, extensions, firstBaseCallProxy);
    }
  }
}
