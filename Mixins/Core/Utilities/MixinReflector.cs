using System;
using System.Reflection;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Utilities
{
  public class MixinReflector
  {
    public enum InitializationMode { Construction, Deserialization }

    public static TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return (TMixin) Get (typeof (TMixin), mixinTarget);
    }

    public static object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      IMixinTarget castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        MixinDefinition mixinDefinition = castMixinTarget.Configuration.GetMixinByConfiguredType (mixinType);
        if (mixinDefinition != null)
          return castMixinTarget.Mixins[mixinDefinition.MixinIndex];
      }
      return null;
    }

    public static Type GetMixinBaseType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type currentType = concreteMixinType;
      Type mixinBaseOne = typeof (Mixin<>);
      Type mixinBaseTwo = typeof (Mixin<,>);

      while (currentType != null && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, mixinBaseOne)
          && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, mixinBaseTwo))
        currentType = currentType.BaseType;
      return currentType;
    }

    public static PropertyInfo GetTargetProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetBaseProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetConfigurationProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static MethodInfo GetInitializationMethod (Type concreteMixinType, InitializationMode initializationMode)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
      {
        string methodName = initializationMode == InitializationMode.Construction ? "Initialize" : "Deserialize";
        MethodInfo method = mixinBaseType.GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return method;
      }
    }

    public static Type GetBaseCallProxyType (object mixinTargetInstance)
    {
      ArgumentUtility.CheckNotNull ("mixinTargetInstance", mixinTargetInstance);
      IMixinTarget castTarget = mixinTargetInstance as IMixinTarget;
      if (castTarget == null)
      {
        string message = string.Format ("The given object of type {0} is not a mixin target.", mixinTargetInstance.GetType().FullName);
        throw new ArgumentException (message, "mixinTargetInstance");
      }

      Assertion.IsNotNull (castTarget.FirstBaseCallProxy);
      Type baseCallProxyType = castTarget.FirstBaseCallProxy.GetType();
      return baseCallProxyType;
    }
  }
}
