using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class SecurityClient
  {
    private ISecurityService _securityService;
    private IPermissionReflector _permissionReflector;

    public SecurityClient ()
        : this (SecurityConfiguration.Current.SecurityService, new PermissionReflector ())
    {
    }

    public SecurityClient (ISecurityService securityService)
        : this (securityService, new PermissionReflector ())
    {
    }

    public SecurityClient (ISecurityService securityService, IPermissionReflector permissionReflector)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("permissionReflector", permissionReflector);
  
      _securityService = securityService;
      _permissionReflector = permissionReflector;
    }

    public bool HasAccess (SecurityContext context, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = _securityService.GetAccess (context, user);

      if (actualAccessTypes == null)
        return false;

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }

    public bool HasAccess (ISecurableType securableType, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);

      ISecurityContextFactory contextFactory = securableType.GetSecurityContextFactory ();
      if (contextFactory == null)
        throw new ArgumentException ("The securable type did not return a ISecurityContextFactory.", "securableType");

      return HasAccess (contextFactory.GetSecurityContext (), user, requiredAccessTypes);
    }

    public bool HasAccess (SecurityContext context, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (context, GetCurrentUser (), requiredAccessTypes);
    }

    public bool HasAccess (ISecurableType securableType, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (securableType, GetCurrentUser (), requiredAccessTypes);
    }

    public void CheckMethodAccess (ISecurableType securableType, string methodName)
    {
      CheckMethodAccess (securableType, methodName, GetCurrentUser ());
    }

    public void CheckMethodAccess (ISecurableType securableType, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredMethodPermissions (securableType.GetType (), methodName);
      CheckRequiredMethodAccess (securableType, methodName, requiredAccessTypeEnums, user);
    }

    public void CheckMethodAccess (ISecurableType securableType, string methodName, Type[] parameterTypes)
    {
      CheckMethodAccess (securableType, methodName, parameterTypes, GetCurrentUser ());
    }

    public void CheckMethodAccess (ISecurableType securableType, string methodName, Type[] parameterTypes, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterTypes", parameterTypes);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredMethodPermissions (securableType.GetType (), methodName, parameterTypes);
      CheckRequiredMethodAccess (securableType, methodName, requiredAccessTypeEnums, user);
    }

    public void CheckConstructorAccess (Type type)
    {
      CheckConstructorAccess (type, GetCurrentUser ());
    }

    public void CheckConstructorAccess (Type type, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredConstructorPermissions (type);
      CheckRequiredConstructorAccess (type, requiredAccessTypeEnums, user);
    }

    public void CheckConstructorAccess (Type type, Type[] parameterTypes)
    {
      CheckConstructorAccess (type, parameterTypes, GetCurrentUser ());
    }

    public void CheckConstructorAccess (Type type, Type[] parameterTypes, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterTypes", parameterTypes);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredConstructorPermissions (type, parameterTypes);
      CheckRequiredConstructorAccess (type, requiredAccessTypeEnums, user);
    }

    private void CheckRequiredConstructorAccess (Type type, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      AccessType[] requiredAccessTypes = ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums);

      if (Array.IndexOf (requiredAccessTypeEnums, GeneralAccessType.Create) < 0)
      {
        Array.Resize (ref requiredAccessTypes, requiredAccessTypes.Length + 1);
        requiredAccessTypes[requiredAccessTypes.Length - 1] = AccessType.Get (GeneralAccessType.Create);
      }

      if (!HasAccess (new SecurityContext (type), user, requiredAccessTypes))
        throw new AccessViolationException ("Access to constructor has been denied.");
    }

    private void CheckRequiredMethodAccess (ISecurableType securableType, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (!HasAccess (securableType, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums)))
        throw new AccessViolationException (string.Format ("Access to method '{0}' has been denied.", methodName));
    }

    private AccessType[] ConvertRequiredAccessTypeEnums (Enum[] requiredAccessTypeEnums)
    {
      return Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }

    private IPrincipal GetCurrentUser ()
    {
      return SecurityConfiguration.Current.UserProvider.GetUser ();
    }
  }
}
