using System;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectSearchService : ISearchAvailableObjectsService
  {
    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      DomainObjectCollection queriedObjects  = ClientTransaction.Current.QueryManager.GetCollection (new Query (searchStatement));
      IBusinessObject[] result = new IBusinessObject[queriedObjects.Count];
      queriedObjects.CopyTo (result, 0);
      return result;
    }
  }
}