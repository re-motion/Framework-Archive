using System;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class GetBindableDomainObjectService : IGetBindableDomainObjectService
  {
    public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
    {
      return (IBusinessObjectWithIdentity) DomainObject.GetObject (ObjectID.Parse (uniqueIdentifier));
    }
  }
}