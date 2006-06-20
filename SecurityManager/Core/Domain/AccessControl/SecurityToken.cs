using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (List<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);
      _abstractRoles = abstractRoles.AsReadOnly ();
    }

    public ReadOnlyCollection<AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }
  }
}
