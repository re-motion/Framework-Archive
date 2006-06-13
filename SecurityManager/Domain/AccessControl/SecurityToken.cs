using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    public static SecurityToken Create (ClientTransaction transaction, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);

      List<AbstractRoleDefinition> abstractRoles = GetAbstractRoles (transaction, context.AbstractRoles);
      return new SecurityToken (abstractRoles);
    }

    private static List<AbstractRoleDefinition> GetAbstractRoles (ClientTransaction transaction, EnumWrapper[] abstractRoleNames)
    {
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();
      DomainObjectCollection abstractRolesCollection = AbstractRoleDefinition.Find (transaction, abstractRoleNames);

      foreach (AbstractRoleDefinition abstractRole in abstractRolesCollection)
        abstractRoles.Add (abstractRole);

      return abstractRoles;
    }

    private ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    private SecurityToken (List<AbstractRoleDefinition> abstractRoles)
    {
      _abstractRoles = abstractRoles.AsReadOnly ();
    }

    public ReadOnlyCollection<AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }
  }
}
