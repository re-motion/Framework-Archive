using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Service
{
  public class SecurityService : ISecurityService
  {
    private ClientTransaction _transaction;
    private IAccessControlListFinder _accessControlListFinder;
    private ISecurityTokenBuilder _securityTokenBuilder;

    public SecurityService ()
      : this (new ClientTransaction())
    {
    }

    public SecurityService (ClientTransaction transaction)
      : this (transaction, new AccessControlListFinder (), new SecurityTokenBuilder ())
    {
    }

    public SecurityService (ClientTransaction transaction, IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull ("securityTokenBuilder", securityTokenBuilder);

      _transaction = transaction;
      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
    }

    public AccessType[] GetAccess (SecurityContext context, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessControlList acl = _accessControlListFinder.Find (_transaction, context);
      SecurityToken token = _securityTokenBuilder.CreateToken (_transaction, context);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);
      return Array.ConvertAll <AccessTypeDefinition, AccessType> (accessTypes, new Converter<AccessTypeDefinition,AccessType> (ConvertToAccessType));
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get (EnumWrapper.Parse (accessTypeDefinition.Name));
    }
  }
}
