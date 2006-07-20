using System;
using System.Collections.Generic;
using System.Security.Principal;

using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class SecurityTokenBuilder : ISecurityTokenBuilder
  {
    /// <exception cref="AccessControlException">
    ///   A matching <see cref="User"/> is not found for the <paramref name="principal"/>.<br/>- or -<br/>
    ///   A matching <see cref="Group"/> is not found for the <paramref name="context"/>'s <see cref="SecurityContext.OwnerGroup"/>.<br/>- or -<br/>
    ///   A matching <see cref="AbstractRoleDefinition"/> is not found for all entries in the <paramref name="context"/>'s <see cref="SecurityContext.AbstractRoles"/> collection.
    /// </exception>
    public SecurityToken CreateToken (ClientTransaction transaction, IPrincipal principal, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("context", context);

      User user = GetUser (transaction, principal.Identity.Name);
      List<Group> owningGroups = GetGroups (transaction, context.OwnerGroup);
      List<AbstractRoleDefinition> abstractRoles = GetAbstractRoles (transaction, context.AbstractRoles);

      return new SecurityToken (user, owningGroups, abstractRoles);
    }

    private User GetUser (ClientTransaction transaction, string userName)
    {
      if (StringUtility.IsNullOrEmpty (userName))
        return null;

      User user = User.FindByUserName (transaction, userName);
      if (user == null)
        throw CreateAccessControlException ("The user '{0}' could not be found.", userName);

      return user;
    }

    private List<Group> GetGroups (ClientTransaction transaction, string groupUniqueIdentifier)
    {
      List<Group> groups = new List<Group> ();

      if (StringUtility.IsNullOrEmpty (groupUniqueIdentifier))
        return groups;

      Group group = Group.FindByUnqiueIdentifier (transaction, groupUniqueIdentifier);
      if (group == null)
        throw CreateAccessControlException ("The group '{0}' could not be found.", groupUniqueIdentifier);

      groups.Add (group);

      while (group.Parent != null)
      {
        group = group.Parent;
        groups.Add (group);
      }

      return groups;
    }

    private List<AbstractRoleDefinition> GetAbstractRoles (ClientTransaction transaction, EnumWrapper[] abstractRoleNames)
    {
      DomainObjectCollection abstractRolesCollection = AbstractRoleDefinition.Find (transaction, abstractRoleNames);
      
      EnumWrapper? missingAbstractRoleName = FindFirstMissingAbstractRole (abstractRoleNames, abstractRolesCollection);
      if (missingAbstractRoleName != null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", missingAbstractRoleName);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();
      foreach (AbstractRoleDefinition abstractRole in abstractRolesCollection)
        abstractRoles.Add (abstractRole);

      return abstractRoles;
    }

    private EnumWrapper? FindFirstMissingAbstractRole (EnumWrapper[] expectedAbstractRoles, DomainObjectCollection actualAbstractRolesCollection)
    {
      if (expectedAbstractRoles.Length == actualAbstractRolesCollection.Count)
        return null;

      AbstractRoleDefinition[] actualAbstractRoles = new AbstractRoleDefinition[actualAbstractRolesCollection.Count];
      actualAbstractRolesCollection.CopyTo (actualAbstractRoles, 0);

      foreach (EnumWrapper expectedAbstractRole in expectedAbstractRoles)
      {
        Predicate<AbstractRoleDefinition> match = delegate (AbstractRoleDefinition current) 
            {
              return current.Name.Equals (expectedAbstractRole.ToString (), StringComparison.Ordinal); 
            };

        if (!Array.Exists<AbstractRoleDefinition> (actualAbstractRoles, match))
          return expectedAbstractRole;
      }

      return null;
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
