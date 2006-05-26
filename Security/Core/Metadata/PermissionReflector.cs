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

      return GetRequiredMethodPermissions (type, methodName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetRequiredMethodPermissions (type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    }

    public Enum[] GetRequiredMethodPermissions (MethodBase methodInfo)
    {
      if (!methodInfo.IsDefined (typeof (DemandMethodPermissionAttribute), true))
        return new Enum[0];

      DemandMethodPermissionAttribute[] demandPermissionAttributes =
          (DemandMethodPermissionAttribute[]) methodInfo.GetCustomAttributes (typeof (DemandMethodPermissionAttribute), true);

      DemandMethodPermissionAttribute demandPermission = demandPermissionAttributes[0];

      List<Enum> permissions = new List<Enum> ();
      foreach (Enum accessTypeEnum in demandPermission.AccessTypes)
      {
        if (!permissions.Contains (accessTypeEnum))
          permissions.Add (accessTypeEnum);
      }

      return permissions.ToArray();
    }

    private bool IsSecuredMethod (MemberInfo member, object filterCriteria)
    {
      string methodName = (string) filterCriteria;
      MethodInfo methodInfo = (MethodInfo) member;

      return methodInfo.Name == methodName && methodInfo.IsDefined (typeof (DemandMethodPermissionAttribute), true);
    }

    private Enum[] GetRequiredMethodPermissions (Type type, string methodName, BindingFlags bindingFlags)
    {
      if (!TypeHasMethod (type, methodName, bindingFlags))
        throw new ArgumentException (string.Format ("The method '{0}' could not be found.", methodName), "methodName");

      MemberInfo[] foundMethods = type.FindMembers (MemberTypes.Method, bindingFlags, IsSecuredMethod, methodName);
      if (foundMethods.Length == 0)
        return new Enum[0];

      if (foundMethods.Length > 1)
      {
        throw new ArgumentException (string.Format (
            "The method '{0}' has multiple RequiredMethodPermissionAttributes defined.", methodName), "methodName");
      }

      MethodInfo foundMethod = (MethodInfo) foundMethods[0];
      if (type.BaseType != null && foundMethod.DeclaringType == type && TypeHasMethod (type.BaseType, methodName, bindingFlags))
      {
        throw new ArgumentException (string.Format (
            "The RequiredMethodPermissionAttributes must not be defined on methods overriden or redefined in derived classes. "
            + "A method '{0}' exists in class '{1}' and its base class.", methodName, type.FullName), "methodName");
      }

      return GetRequiredMethodPermissions (foundMethod);
    }

    private bool TypeHasMethod (Type type, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, MemberTypes.Method, bindingFlags);
      return existingMembers.Length > 0;
    }
  }
}
