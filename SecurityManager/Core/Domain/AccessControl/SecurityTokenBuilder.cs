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
    public SecurityToken CreateToken (ClientTransaction transaction, IPrincipal principal, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);

      User user = GetUser (transaction, principal.Identity.Name);
      List<Group> groups = GetGroups (transaction, context.OwnerGroup);
      List<AbstractRoleDefinition> abstractRoles = GetAbstractRoles (transaction, context.AbstractRoles);

      return new SecurityToken (user, groups, abstractRoles);
    }

    private User GetUser (ClientTransaction transaction, string userName)
    {
      User user = User.Find (transaction, userName);
      if (user == null)
        throw new ArgumentException (string.Format ("The user '{0}' could not be found.", userName), "userName");

      return user;
    }

    private List<Group> GetGroups (ClientTransaction transaction, string groupName)
    {
      List<Group> groups = new List<Group> ();

      Group group = Group.Find (transaction, groupName);
      if (group == null)
        throw new ArgumentException (string.Format ("The group '{0}' could not be found.", groupName), "groupName");

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
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();
      DomainObjectCollection abstractRolesCollection = AbstractRoleDefinition.Find (transaction, abstractRoleNames);

      foreach (AbstractRoleDefinition abstractRole in abstractRolesCollection)
        abstractRoles.Add (abstractRole);

      return abstractRoles;
    }
  }
}
