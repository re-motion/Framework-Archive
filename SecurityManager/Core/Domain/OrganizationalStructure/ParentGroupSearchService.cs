using System;
using Rubicon.Data.DomainObjects;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public class ParentGroupSearchService : ISearchAvailableObjectsService
  {
    public ParentGroupSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      Group group = ArgumentUtility.CheckNotNullAndType<Group> ("referencingObject", referencingObject);
      return (IBusinessObject[])ArrayUtility.Convert (group.GetPossibleParentGroups (group.Tenant.ID), typeof (IBusinessObject));
    }
  }
}