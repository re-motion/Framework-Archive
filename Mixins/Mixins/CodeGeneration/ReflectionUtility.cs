using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
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
      Type currentType = concreteMixinType;
      Type mixinBaseOne = typeof (Mixin<>);
      Type mixinBaseTwo = typeof (Mixin<,>);

      while (currentType != null && !IsEqualOrInstantiationOf (currentType, mixinBaseOne) && !IsEqualOrInstantiationOf (currentType, mixinBaseTwo))
        currentType = currentType.BaseType;
      return currentType;
    }

    public static bool IsEqualOrInstantiationOf (Type typeToCheck, Type expectedType)
    {
      return typeToCheck.Equals (expectedType) || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition().Equals (expectedType));
    }

    public static MethodInfo GetInitializationMethod (Type concreteMixinType)
    {
      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetTargetProperty (Type concreteMixinType)
    {
      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetBaseProperty (Type concreteMixinType)
    {
      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}
