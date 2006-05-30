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
    public static SecurityClient CreateSecurityClientFromConfiguration ()
    {
      ISecurityService securityService = SecurityConfiguration.Current.SecurityService;
      IPermissionProvider permissionProvider = SecurityConfiguration.Current.PermissionProvider;
      IUserProvider userProvider = SecurityConfiguration.Current.UserProvider;
      IFunctionalSecurityStrategy functionalSecurityStrategy = SecurityConfiguration.Current.FunctionalSecurityStrategy;

      if (securityService == null)
        throw new SecurityConfigurationException ("The security service has not been configured.");

      if (userProvider == null)
        throw new SecurityConfigurationException ("The user provider has not been configured.");

      return new SecurityClient (securityService, permissionProvider, userProvider, functionalSecurityStrategy);
    }

    private ISecurityService _securityService;
    private IPermissionProvider _permissionProvider;
    private IUserProvider _userProvider;
    private IFunctionalSecurityStrategy _functionalSecurityStrategy;

    public SecurityClient (
        ISecurityService securityService,
        IPermissionProvider permissionReflector,
        IUserProvider userProvider,
        IFunctionalSecurityStrategy functionalSecurityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("permissionReflector", permissionReflector);
      ArgumentUtility.CheckNotNull ("userProvider", userProvider);
      ArgumentUtility.CheckNotNull ("functionalSecurityStrategy", functionalSecurityStrategy);

      _securityService = securityService;
      _permissionProvider = permissionReflector;
      _userProvider = userProvider;
      _functionalSecurityStrategy = functionalSecurityStrategy;
    }

    public bool HasAccess (ISecurableObject securableObject, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      if (objectSecurityStrategy == null)
        throw new ArgumentException ("The securable object did not return a IObjectSecurityStrategy.", "securableObject");

      return objectSecurityStrategy.HasAccess (_securityService, user, requiredAccessTypes);
    }

    public bool HasAccess (ISecurableObject securableType, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (securableType, _userProvider.GetUser (), requiredAccessTypes);
    }

    public void CheckMethodAccess (ISecurableObject securableType, string methodName)
    {
      CheckMethodAccess (securableType, methodName, _userProvider.GetUser ());
    }

    public void CheckMethodAccess (ISecurableObject securableType, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableType.GetType (), methodName);
      CheckRequiredMethodAccess (securableType, methodName, requiredAccessTypeEnums, user);
    }

    public void CheckConstructorAccess (Type type)
    {
      CheckConstructorAccess (type, _userProvider.GetUser ());
    }

    public void CheckConstructorAccess (Type type, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessType[] requiredAccessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create) };

      if (!_functionalSecurityStrategy.HasAccess (type, _securityService, user, requiredAccessTypes))
        throw new PermissionDeniedException (string.Format ("Access to constructor for type '{0}' has been denied.", type.FullName));
   }

    public void CheckStaticMethodAccess (Type type, string methodName)
    {
      CheckStaticMethodAccess (type, methodName, _userProvider.GetUser ());
    }

    public void CheckStaticMethodAccess (Type type, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredStaticMethodPermissions (type, methodName);
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

      if (!_functionalSecurityStrategy.HasAccess (type, _securityService, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums)))
        throw new PermissionDeniedException (string.Format ("Access to static method '{0}' on type '{1}' has been denied.", methodName, type.FullName));
    }

    private AccessType[] ConvertRequiredAccessTypeEnums (Enum[] requiredAccessTypeEnums)
    {
      return Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }
  }
}
