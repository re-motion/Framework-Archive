using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{
  public class PermissionReflector : IPermissionProvider
  {
    private const string c_memberNotFoundMessage = "The member '{0}' could not be found.";
    private const string c_memberHasMultipleAttributesMessage = "The member '{0}' has multiple {1} defined.";
    private const string c_memberPermissionsOnlyInBaseClassMessage = "The {2} must not be defined on members overriden or redefined in derived classes. "
      + "A member '{0}' exists in class '{1}' and its base class.";

    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetRequiredPermissions<DemandMethodPermissionAttribute> (type, methodName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetRequiredPermissions<DemandMethodPermissionAttribute> (type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetRequiredPermissions<DemandPropertyReadPermissionAttribute> (type, propertyName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetRequiredPermissions<DemandPropertyWritePermissionAttribute> (type, propertyName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetPermissions<T> (MemberInfo methodInfo) where T : BaseDemandPermissionAttribute
    {
      if (!methodInfo.IsDefined (typeof (T), true))
        return new Enum[0];

      T[] permissionAttributes = (T[]) methodInfo.GetCustomAttributes (typeof (T), true);

      T permission = permissionAttributes[0];

      List<Enum> permissions = new List<Enum> ();
      foreach (Enum accessTypeEnum in permission.AccessTypes)
      {
        if (!permissions.Contains (accessTypeEnum))
          permissions.Add (accessTypeEnum);
      }

      return permissions.ToArray ();
    }

    private bool IsSecuredMethod<T> (MemberInfo member, object filterCriteria) where T : BaseDemandPermissionAttribute
    {
      string memberName = (string) filterCriteria;
      return member.Name == memberName && member.IsDefined (typeof (T), true);
    }

    private Enum[] GetRequiredPermissions<T> (Type type, string memberName, BindingFlags bindingFlags) where T : BaseDemandPermissionAttribute
    {
      MemberTypes memberType = GetApplicableMemberTypesFromAttributeType (typeof (T));
      string attributeName = typeof (T).Name;

      if (!TypeHasMember (type, memberType, memberName, bindingFlags))
        throw new ArgumentException (string.Format (c_memberNotFoundMessage, memberName), "memberName");

      MemberInfo[] foundMembers = type.FindMembers (memberType, bindingFlags, IsSecuredMethod<T>, memberName);
      if (foundMembers.Length == 0)
        return new Enum[0];

      if (foundMembers.Length > 1)
        throw new ArgumentException (string.Format (c_memberHasMultipleAttributesMessage, memberName, attributeName), "memberName");

      MemberInfo foundMember = (MemberInfo) foundMembers[0];
      if (type.BaseType != null && foundMember.DeclaringType == type && TypeHasMember (type.BaseType, memberType, memberName, bindingFlags))
        throw new ArgumentException (string.Format (c_memberPermissionsOnlyInBaseClassMessage, memberName, type.FullName, attributeName), "memberName");

      return GetPermissions<T> (foundMember);
    }

    private bool TypeHasMember (Type type, MemberTypes memberType, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, memberType, bindingFlags);
      return existingMembers.Length > 0;
    }

    private MemberTypes GetApplicableMemberTypesFromAttributeType (Type attributeType)
    {
      AttributeUsageAttribute[] attributeUsageAttributes = (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), false);
      AttributeTargets targets = attributeUsageAttributes[0].ValidOn;

      MemberTypes memberTypes = 0;

      if ((targets & AttributeTargets.Method) != 0)
        memberTypes |= MemberTypes.Method;

      if ((targets & AttributeTargets.Property) != 0)
        memberTypes |= MemberTypes.Property;

      return memberTypes;
    }
  }
}
