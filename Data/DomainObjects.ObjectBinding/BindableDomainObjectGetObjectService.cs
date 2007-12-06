using System;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectGetObjectService : IGetObjectService
  {
    public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classWithIdentity", classWithIdentity);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      return (IBusinessObjectWithIdentity) RepositoryAccessor.GetObject (ObjectID.Parse (uniqueIdentifier), false);
    }
  }
}