using System;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  public interface IBusinessObjectClassService : IBusinessObjectService
  {
    IBusinessObjectClass GetBusinessObjectClass (Type type);
  }
}