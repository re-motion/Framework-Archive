using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IGetObjectService : IBusinessObjectService
  {
    IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier);
  }
}