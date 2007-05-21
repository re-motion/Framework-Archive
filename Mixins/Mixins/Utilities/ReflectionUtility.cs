using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Collections;

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

    public static bool IsEqualOrInstantiationOf (Type typeToCheck, Type expectedType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("expectedType", expectedType);

      return typeToCheck.Equals (expectedType) || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition().Equals (expectedType));
    }

    public static bool IsNewSlotMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.NewSlot);
    }

    public static bool IsVirtualMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.Virtual);
    }

    private static bool CheckMethodAttributeOnMember (MemberInfo member, MethodAttributes attribute)
    {
      MethodInfo method = member as MethodInfo;
      if (method != null)
        return (method.Attributes & attribute) == attribute;

      PropertyInfo property = member as PropertyInfo;
      if (property != null)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);
        return (getMethod != null && CheckMethodAttributeOnMember (getMethod, attribute))
            || (setMethod != null && CheckMethodAttributeOnMember (setMethod, attribute));
      }

      EventInfo eventInfo = member as EventInfo;
      if (eventInfo != null)
        return CheckMethodAttributeOnMember(eventInfo.GetAddMethod (), attribute)
            || CheckMethodAttributeOnMember(eventInfo.GetRemoveMethod (), attribute);

      string message = string.Format (
          "The given member {0}.{1} is neither property, method, nor event.",
          member.DeclaringType.FullName,
          member.Name);
      throw new ArgumentException (message, "member");
    }

    public static IEnumerable<MethodInfo> RecursiveGetAllMethods (Type type, BindingFlags bindingFlags)
    {
      foreach (MethodInfo method in type.GetMethods(bindingFlags | BindingFlags.DeclaredOnly))
        yield return method;

      if (type.BaseType != null)
      {
        foreach (MethodInfo method in RecursiveGetAllMethods (type.BaseType, bindingFlags))
          yield return method;
      }
    }

    public static IEnumerable<PropertyInfo> RecursiveGetAllProperties (Type type, BindingFlags bindingFlags)
    {
      foreach (PropertyInfo property in type.GetProperties (bindingFlags | BindingFlags.DeclaredOnly))
        yield return property;

      if (type.BaseType != null)
      {
        foreach (PropertyInfo property in RecursiveGetAllProperties (type.BaseType, bindingFlags))
          yield return property;
      }
    }

    public static IEnumerable<EventInfo> RecursiveGetAllEvents (Type type, BindingFlags bindingFlags)
    {
      foreach (EventInfo eventInfo in type.GetEvents (bindingFlags | BindingFlags.DeclaredOnly))
        yield return eventInfo;

      if (type.BaseType != null)
      {
        foreach (EventInfo eventInfo in RecursiveGetAllEvents (type.BaseType, bindingFlags))
          yield return eventInfo;
      }
    }

    public static Type[] GetMethodParameterTypes(MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      ParameterInfo[] parameters = method.GetParameters();
      Type[] parameterTypes = new Type[parameters.Length];
      for (int i = 0; i < parameterTypes.Length; ++i)
        parameterTypes[i] = parameters[i].ParameterType;
      
      return parameterTypes;
    }

    public static Tuple<Type, Type[]> GetMethodSignature (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      
      Type[] parameterTypes = GetMethodParameterTypes (method);
      return new Tuple<Type, Type[]> (method.ReturnType, parameterTypes);
    }

  }
}