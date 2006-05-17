using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{
  public class PermissionReflector : IPermissionReflector
  {
    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodInfo methodInfo = type.GetMethod (methodName, BindingFlags.Public | BindingFlags.Instance);
      if (methodInfo == null)
        throw new ArgumentException (string.Format ("The method '{0}' does not exist on type '{1}'.", methodName, type.FullName), "methodName");

      return GetRequiredMethodPermissions (methodInfo);
    }

    public Enum[] GetRequiredMethodPermissions (Type type, string methodName, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterTypes", parameterTypes);

      MethodInfo methodInfo = type.GetMethod (methodName, BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null);
      if (methodInfo == null)
        throw new ArgumentException (string.Format ("The method '{0}' does not exist on type '{1}'.", methodName, type.FullName), "methodName");

      return GetRequiredMethodPermissions (methodInfo);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodInfo methodInfo = type.GetMethod (methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
      if (methodInfo == null)
        throw new ArgumentException (string.Format ("The static method '{0}' does not exist on type '{1}'.", methodName, type.FullName), "methodName");

      return GetRequiredMethodPermissions (methodInfo);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterTypes", parameterTypes);

      MethodInfo methodInfo = type.GetMethod (methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null,
          parameterTypes, null);

      if (methodInfo == null)
        throw new ArgumentException (string.Format ("The static method '{0}' does not exist on type '{1}'.", methodName, type.FullName), "methodName");

      return GetRequiredMethodPermissions (methodInfo);
    }

    public Enum[] GetRequiredConstructorPermissions (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ConstructorInfo[] constructorInfos = type.GetConstructors (BindingFlags.Public | BindingFlags.Instance);
      if (constructorInfos.Length > 1)
        throw new AmbiguousMatchException ();

      if (constructorInfos.Length == 0)
        throw new ArgumentException (string.Format ("Type '{0}' does not have a public constructor.", type.FullName), "type");

      return GetRequiredMethodPermissions (constructorInfos[0]);
    }

    public Enum[] GetRequiredConstructorPermissions (Type type, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterTypes", parameterTypes);

      ConstructorInfo constructorInfo = type.GetConstructor (BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null);
      if (constructorInfo == null)
        throw new ArgumentException (string.Format ("The specified constructor for type '{0}' is not public.", type.FullName), "type");

      return GetRequiredMethodPermissions (constructorInfo);
    }

    public Enum[] GetRequiredMethodPermissions (MethodBase methodInfo)
    {
      if (!methodInfo.IsDefined (typeof (RequiredMethodPermissionAttribute), true))
        return new Enum[0];

      RequiredMethodPermissionAttribute[] requiredPermissionAttributes =
          (RequiredMethodPermissionAttribute[]) methodInfo.GetCustomAttributes (typeof (RequiredMethodPermissionAttribute), true);

      List<Enum> requiredPermissions = new List<Enum> ();

      foreach (RequiredMethodPermissionAttribute requiredPermissionAttribute in requiredPermissionAttributes)
      {
        if (!requiredPermissions.Contains (requiredPermissionAttribute.AccessType))
          requiredPermissions.Add (requiredPermissionAttribute.AccessType);
      }

      return requiredPermissions.ToArray ();
    }
  }
}
