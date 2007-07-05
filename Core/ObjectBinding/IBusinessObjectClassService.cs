using System;

namespace Rubicon.ObjectBinding
{
  //TODO: doc
  public interface IBusinessObjectClassService : IBusinessObjectService
  {
    IBusinessObjectClass GetBusinessObjectClass (Type type);
  }
}