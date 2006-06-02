using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;
using System.ComponentModel;

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


    public bool HasAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (securableObject, _userProvider.GetUser (), requiredAccessTypes);
    }

    public bool HasAccess (ISecurableObject securableObject, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      if (objectSecurityStrategy == null)
        throw new ArgumentException ("The securable object did not return a IObjectSecurityStrategy.", "securableObject");

      return objectSecurityStrategy.HasAccess (_securityService, user, requiredAccessTypes);
    }


    public bool HasMethodAccess (ISecurableObject securableObject, string methodName)
    {
      return HasMethodAccess (securableObject, methodName, _userProvider.GetUser ());
    }

    public bool HasMethodAccess (ISecurableObject securableObject, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableObject.GetType (), methodName);
      return HasRequiredMethodAccess (securableObject, methodName, requiredAccessTypeEnums, user);
    }

    public bool HasConstructorAccess (Type securableClass)
    {
      return HasConstructorAccess (securableClass, _userProvider.GetUser ());
    }

    public bool HasConstructorAccess (Type securableClass, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessType[] requiredAccessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create) };

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityService, user, requiredAccessTypes);
    }

    public bool HasStaticMethodAccess (Type securableClass, string methodName)
    {
      return HasStaticMethodAccess (securableClass, methodName, _userProvider.GetUser ());
    }

    public bool HasStaticMethodAccess (Type securableClass, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredStaticMethodPermissions (securableClass, methodName);
      return HasRequiredMethodAccess (securableClass, methodName, requiredAccessTypeEnums, user);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, string methodName)
    {
      return HasStatelessMethodAccess (securableClass, methodName, _userProvider.GetUser ());
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableClass, methodName);
      return HasRequiredMethodAccess (securableClass, methodName, requiredAccessTypeEnums, user);
    }

    private bool HasRequiredMethodAccess (ISecurableObject securableObject, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The method '{0}' does not define required permissions.", methodName), "requiredAccessTypeEnums");

      return HasAccess (securableObject, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }

    private bool HasRequiredMethodAccess (Type securableClass, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The method '{0}' does not define required permissions.", methodName), "requiredAccessTypeEnums");

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityService, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }


    public void CheckMethodAccess (ISecurableObject securableObject, string methodName)
    {
      CheckMethodAccess (securableObject, methodName, _userProvider.GetUser ());
    }

    public void CheckMethodAccess (ISecurableObject securableObject, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableObject.GetType (), methodName);
      CheckRequiredMethodAccess (securableObject, methodName, requiredAccessTypeEnums, user);
    }

    public void CheckConstructorAccess (Type securableClass)
    {
      CheckConstructorAccess (securableClass, _userProvider.GetUser ());
    }

    public void CheckConstructorAccess (Type securableClass, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);

      if (!HasConstructorAccess (securableClass, user))
        throw new PermissionDeniedException (string.Format ("Access to constructor for type '{0}' has been denied.", securableClass.FullName));
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName)
    {
      CheckStaticMethodAccess (securableClass, methodName, _userProvider.GetUser ());
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredStaticMethodPermissions (securableClass, methodName);
      CheckRequiredMethodAccess (securableClass, methodName, requiredAccessTypeEnums, user);
    }

    private void CheckRequiredMethodAccess (ISecurableObject securableObject, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (!HasRequiredMethodAccess (securableObject, methodName, requiredAccessTypeEnums, user))
      {
        throw new PermissionDeniedException (string.Format (
            "Access to method '{0}' on type '{1}' has been denied.", methodName, securableObject.GetType ().FullName));
      }
    }

    private void CheckRequiredMethodAccess (Type securableClass, string methodName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (!HasRequiredMethodAccess (securableClass, methodName, requiredAccessTypeEnums, user))
      {
        throw new PermissionDeniedException (string.Format (
            "Access to static method '{0}' on type '{1}' has been denied.", methodName, securableClass.FullName));
      }

    }


    private AccessType[] ConvertRequiredAccessTypeEnums (Enum[] requiredAccessTypeEnums)
    {
      return Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }
  }
}
