using System;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public class RolePropertiesSearchService : ISearchAvailableObjectsService
  {
    public RolePropertiesSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (property.Identifier == "Group" || property.Identifier == "User")
        return true;
      else
        return false;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      Role role = ArgumentUtility.CheckNotNullAndType<Role> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case "Group":
          if (role.User == null || role.User.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (role.GetPossibleGroups (role.User.Tenant.ID), typeof (IBusinessObject));
        case "User":
          if (role.Group == null || role.Group.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (User.FindByTenantID (role.Group.Tenant.ID), typeof (IBusinessObject));
        default:
          throw new ArgumentException (
              string.Format (
                  "The property '{0}' is not supported by the '{1}' type.", property.DisplayName, typeof (RolePropertiesSearchService).FullName));
      }
    }
  }
}