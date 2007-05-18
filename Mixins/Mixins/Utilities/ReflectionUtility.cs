using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Rubicon.Utilities;
using Mixins.Definitions;

namespace Mixins.Utilities
{
  public static class ReflectionUtility
  {
    public static Type FindGenericTypeDefinitionInClosedHierarchy (Type typeContainingGenericParams, Type closedTypeToSeach)
    {
      ArgumentUtility.CheckNotNull ("typeContainingGenericParams", typeContainingGenericParams);
      ArgumentUtility.CheckNotNull ("closedTypeToSeach", closedTypeToSeach);

      if (!typeContainingGenericParams.ContainsGenericParameters)
        throw new ArgumentException ("Must contain generic parameters.", "typeContainingGenericParams");
      if (closedTypeToSeach.IsGenericTypeDefinition)
        throw new ArgumentException ("Must be closed type or non-generic type.", "closedTypeToSearch");

      if (!typeContainingGenericParams.IsGenericTypeDefinition)
        typeContainingGenericParams = typeContainingGenericParams.GetGenericTypeDefinition();

      if (closedTypeToSeach.IsGenericType && closedTypeToSeach.GetGenericTypeDefinition ().Equals (typeContainingGenericParams))
        return closedTypeToSeach;
      else
      {
        Type baseType = closedTypeToSeach.BaseType;
        if (baseType == null)
          return null;
        else
          return FindGenericTypeDefinitionInClosedHierarchy (typeContainingGenericParams, baseType);
      }
    }

    public static MethodInfo MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (MethodInfo methodToMap, Type closedTypeToSeach)
    {
      ArgumentUtility.CheckNotNull ("methodToMap", methodToMap);
      ArgumentUtility.CheckNotNull ("closedTypeToSeach", closedTypeToSeach);

      if (!methodToMap.DeclaringType.ContainsGenericParameters)
        throw new ArgumentException ("Must be declared by a type containing generic parameters.", "methodToMap");

      Type declaringType = FindGenericTypeDefinitionInClosedHierarchy (methodToMap.DeclaringType, closedTypeToSeach);
      if (declaringType != null)
      {
        return declaringType.GetMethod (methodToMap.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      }
      else
      {
        return null;
      }
    }

    public static Type GetMixinBaseType(Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type currentType = concreteMixinType;
      Type mixinBaseOne = typeof (Mixin<>);
      Type mixinBaseTwo = typeof (Mixin<,>);

      while (currentType != null && !IsEqualOrInstantiationOf (currentType, mixinBaseOne) && !IsEqualOrInstantiationOf (currentType, mixinBaseTwo))
        currentType = currentType.BaseType;
      return currentType;
    }

    public static bool IsEqualOrInstantiationOf (Type typeToCheck, Type expectedType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("expectedType", expectedType);

      return typeToCheck.Equals (expectedType) || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition().Equals (expectedType));
    }

    public static MethodInfo GetInitializationMethod (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
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

    public static IEnumerable<MethodInfo> GetAllInstanceMethodBaseDefinitions(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      //Set<PropertyInfo> returnedProperties = new Set<PropertyInfo>();
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
      foreach (MethodInfo methodInfo in type.GetMethods (bindingFlags))
      {
        if (methodInfo.GetBaseDefinition() == methodInfo) // only collect base definitions
          yield return methodInfo;
      }
      if (type.BaseType != null)
      {
        foreach (MethodInfo methodInfo in GetAllInstanceMethodBaseDefinitions (type.BaseType))
          yield return methodInfo;
      }
    }

    public static IEnumerable<PropertyInfo> GetAllInstancePropertyBaseDefinitions (Type type)
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
      return type.GetProperties (bindingFlags);
      /*foreach (PropertyInfo propertyInfo in type.GetProperties (bindingFlags))
      {
        // if (propertyInfo.GetBaseDefinition () == propertyInfo) // only collect base definitions
          yield return propertyInfo;
      }
      if (type.BaseType != null)
      {
        foreach (PropertyInfo propertyInfo in GetAllInstancePropertyBaseDefinitions (type.BaseType))
          yield return propertyInfo;
      }*/
    }

    public static IEnumerable<EventInfo> GetAllInstanceEventBaseDefinitions (Type type)
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
      return type.GetEvents (bindingFlags);
    }
  }
}