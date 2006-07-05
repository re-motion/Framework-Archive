using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager
{
  public class SecurityService : ISecurityService
  {
    private IAccessControlListFinder _accessControlListFinder;
    private ISecurityTokenBuilder _securityTokenBuilder;

    public SecurityService ()
      : this (new AccessControlListFinder (), new SecurityTokenBuilder ())
    {
    }

    public SecurityService (IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
    {
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull ("securityTokenBuilder", securityTokenBuilder);

      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
    }

    public AccessType[] GetAccess (SecurityContext context, IPrincipal user)
    {
      return GetAccess (new ClientTransaction (), context, user);
    }

    public AccessType[] GetAccess (ClientTransaction transaction, SecurityContext context, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessControlList acl = _accessControlListFinder.Find (transaction, context);
      SecurityToken token = _securityTokenBuilder.CreateToken (transaction, user, context);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);
      return Array.ConvertAll <AccessTypeDefinition, AccessType> (accessTypes, new Converter<AccessTypeDefinition,AccessType> (ConvertToAccessType));
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get (EnumWrapper.Parse (accessTypeDefinition.Name));
    }
  }
}
