using System;
using System.Collections.Specialized;
using System.Security.Principal;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects;
using Rubicon.Logging;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager
{
  public class SecurityService: ExtendedProviderBase, ISecurityProvider
  {
    private static ILog s_log = LogManager.GetLogger (typeof (SecurityService));

    private IAccessControlListFinder _accessControlListFinder;
    private ISecurityTokenBuilder _securityTokenBuilder;

    public SecurityService()
        : this (new AccessControlListFinder(), new SecurityTokenBuilder())
    {
    }

    public SecurityService (IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
        : this ("SecurityManager", new NameValueCollection(), accessControlListFinder, securityTokenBuilder)
    {
    }


    public SecurityService (string name, NameValueCollection config)
        : this (name, config, new AccessControlListFinder(), new SecurityTokenBuilder())
    {
    }

    public SecurityService (
        string name, NameValueCollection config, IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull ("securityTokenBuilder", securityTokenBuilder);

      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
    }

    public AccessType[] GetAccess (SecurityContext context, IPrincipal user)
    {
      return GetAccess (ClientTransaction.NewTransaction(), context, user);
    }

    public AccessType[] GetAccess (ClientTransaction transaction, SecurityContext context, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessControlList acl;
      SecurityToken token;
      try
      {
        acl = _accessControlListFinder.Find (transaction, context);
        token = _securityTokenBuilder.CreateToken (transaction, user, context);
      }
      catch (AccessControlException e)
      {
        s_log.Error ("Error during evaluation of security query.", e);
        return new AccessType[0];
      }

      using (transaction.EnterScope())
      {
        AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);
        return Array.ConvertAll<AccessTypeDefinition, AccessType> (accessTypes, ConvertToAccessType);
      }
    }

    public int GetRevision ()
    {
      using (new ClientTransactionScope())
      {
        return Revision.GetRevision ();
      }
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get (EnumWrapper.Parse (accessTypeDefinition.Name));
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}