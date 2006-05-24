using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;

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

    public bool HasAccess (ISecurableObject securableType, IPrincipal user, params AccessType[] requiredAccessTypes)
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

    public bool HasAccess (ISecurableObject securableType, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (securableType, GetCurrentUser (), requiredAccessTypes);
    }

    public void CheckMethodAccess (ISecurableObject securableType, string methodName)
    {
      CheckMethodAccess (securableType, methodName, GetCurrentUser ());
    }

    public void CheckMethodAccess (ISecurableObject securableType, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredMethodPermissions (securableType.GetType (), methodName);
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

      AccessType[] requiredAccessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create) };

      if (!HasAccess (new SecurityContext (type), user, requiredAccessTypes ))
        throw new PermissionDeniedException (string.Format ("Access to constructor for type '{0}' has been denied.", type.FullName));
   }

    public void CheckStaticMethodAccess (Type type, string methodName)
    {
      CheckStaticMethodAccess (type, methodName, GetCurrentUser ());
    }

    public void CheckStaticMethodAccess (Type type, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Enum[] requiredAccessTypeEnums = _permissionReflector.GetRequiredStaticMethodPermissions (type, methodName);
      CheckRequiredStaticMethodAccess (type, methodName, requiredAccessTypeEnums, user);
    }

    private void CheckRequiredMethodAccess (ISecurableObject securableType, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The method '{0}' does not define required permissions.", methodName), "requiredAccessTypeEnums");

      if (!HasAccess (securableType, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums)))
      {
        throw new PermissionDeniedException (string.Format (
            "Access to method '{0}' on type '{1}' has been denied.", methodName, securableType.GetType ().FullName));
      }
    }

    private void CheckRequiredStaticMethodAccess (Type type, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The method '{0}' does not define required permissions.", methodName), "requiredAccessTypeEnums");

      if (!HasAccess (new SecurityContext (type), user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums)))
        throw new PermissionDeniedException (string.Format ("Access to static method '{0}' on type '{1}' has been denied.", methodName, type.FullName));
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
