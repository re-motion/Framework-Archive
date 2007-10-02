using System;
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

      return (IBusinessObjectWithIdentity) DomainObject.GetObject (ObjectID.Parse (uniqueIdentifier));
    }
  }
}