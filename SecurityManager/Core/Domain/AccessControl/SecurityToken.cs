using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private User _user;
    private ReadOnlyCollection<Group> _groups;
    private ReadOnlyCollection<Role> _roles;
    private ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (User user, List<Group> groups, List<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("groups", groups);
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _user = user;
      _groups = groups.AsReadOnly ();
      _roles = GetRoles (_user, _groups);
      _abstractRoles = abstractRoles.AsReadOnly ();
    }

    public User User
    {
      get { return _user; }
    }

    public ReadOnlyCollection<Group> Groups
    {
      get { return _groups; }
    }

    public ReadOnlyCollection<Role> Roles
    {
      get { return _roles; }
    }

    public ReadOnlyCollection<AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public bool ContainsRoleForPosition (Position position)
    {
      ArgumentUtility.CheckNotNull ("position", position);

      foreach (Role role in _roles)
      {
        if (role.Position.ID == position.ID)
          return true;
      }

      return false;
    }

    private ReadOnlyCollection<Role> GetRoles (User user, IEnumerable<Group> groups)
    {
      List<Role> roles = new List<Role> ();

      foreach (Group group in groups)
        roles.AddRange (user.GetRolesForGroup (group));

      return roles.AsReadOnly ();
    }
  }
}
