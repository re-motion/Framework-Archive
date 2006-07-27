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

      if (SecurityFreeSection.IsActive)
        return true;

      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      if (objectSecurityStrategy == null)
        throw new InvalidOperationException ("The securableObject did not return an IObjectSecurityStrategy.");

      return objectSecurityStrategy.HasAccess (_securityService, user, requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      CheckAccess (securableObject, _userProvider.GetUser (), requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      if (!HasAccess (securableObject, user, requiredAccessTypes))
        throw CreatePermissionDeniedException ("Access has been denied.");
    }


    public bool HasStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      return HasStatelessAccess (securableClass, _userProvider.GetUser (), requiredAccessTypes);
    }

    public bool HasStatelessAccess (Type securableClass, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      if (SecurityFreeSection.IsActive)
        return true;

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityService, user, requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      CheckStatelessAccess (securableClass, _userProvider.GetUser (), requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      if (!HasStatelessAccess (securableClass, user, requiredAccessTypes))
        throw CreatePermissionDeniedException ("Access has been denied.");
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
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasAccess (securableObject, methodName, requiredAccessTypeEnums, user);
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

      if (!HasMethodAccess (securableObject, methodName, user))
        throw CreatePermissionDeniedException ("Access to method '{0}' on type '{1}' has been denied.", methodName, securableObject.GetType ().FullName);
    }


    public bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyReadAccess (securableObject, propertyName, _userProvider.GetUser ());
    }

    public bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredPropertyReadPermissions (securableObject.GetType (), propertyName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredPropertyReadPermissions evaluated and returned null.");
      
      if (requiredAccessTypeEnums.Length == 0)
        requiredAccessTypeEnums = new Enum[] { GeneralAccessType.Read };

      return HasAccess (securableObject, propertyName, requiredAccessTypeEnums, user);
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyReadAccess (securableObject, propertyName, _userProvider.GetUser ());
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      if (!HasPropertyReadAccess (securableObject, propertyName, user))
        throw CreatePermissionDeniedException ("Access to get-accessor of property '{0}' on type '{1}' has been denied.", propertyName, securableObject.GetType ().FullName);
    }


    public bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyWriteAccess (securableObject, propertyName, _userProvider.GetUser ());
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredPropertyWritePermissions (securableObject.GetType (), propertyName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.");
      
      if (requiredAccessTypeEnums.Length == 0)
        requiredAccessTypeEnums = new Enum[] { GeneralAccessType.Edit };

      return HasAccess (securableObject, propertyName, requiredAccessTypeEnums, user);
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyWriteAccess (securableObject, propertyName, _userProvider.GetUser ());
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      if (!HasPropertyWriteAccess (securableObject, propertyName, user))
        throw CreatePermissionDeniedException ("Access to set-accessor of property '{0}' on type '{1}' has been denied.", propertyName, securableObject.GetType ().FullName);
    }


    public bool HasConstructorAccess (Type securableClass)
    {
      return HasConstructorAccess (securableClass, _userProvider.GetUser ());
    }

    public bool HasConstructorAccess (Type securableClass, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);

      if (SecurityFreeSection.IsActive)
        return true;

      AccessType[] requiredAccessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create) };

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityService, user, requiredAccessTypes);
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
        throw CreatePermissionDeniedException ("Access to constructor of type '{0}' has been denied.", securableClass.FullName);
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
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredStaticMethodPermissions evaluated and returned null.");
      
      return HasStatelessAccess (securableClass, methodName, requiredAccessTypeEnums, user);
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

      if (!HasStaticMethodAccess (securableClass, methodName, user))
        throw CreatePermissionDeniedException ("Access to static method '{0}' on type '{1}' has been denied.", methodName, securableClass.FullName);
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
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");
      
      return HasStatelessAccess (securableClass, methodName, requiredAccessTypeEnums, user);
    }


    private bool HasAccess (ISecurableObject securableObject, string memberName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The member '{0}' does not define required permissions.", memberName), "requiredAccessTypeEnums");

      return HasAccess (securableObject, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }

    private bool HasStatelessAccess (Type securableClass, string memberName, Enum[] requiredAccessTypeEnums, IPrincipal user)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The member '{0}' does not define required permissions.", memberName), "requiredAccessTypeEnums");

      return HasStatelessAccess (securableClass, user, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }


    private AccessType[] ConvertRequiredAccessTypeEnums (Enum[] requiredAccessTypeEnums)
    {
      return Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }

    private PermissionDeniedException CreatePermissionDeniedException (string message, params object[] args)
    {
      return new PermissionDeniedException (string.Format (message, args));
    }
  }
}
