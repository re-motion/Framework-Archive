using System;
using System.Collections;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   Provides functionality for business object providers.
/// </summary>
/// <remarks>
///   Note that if this interface is implemented using singletons, the singleton must be thread save.
/// </remarks>
public interface IBusinessObjectProvider
{
  IBusinessObjectService GetService (Type serviceType);
}

public abstract class BusinessObjectProvider: IBusinessObjectProvider
{
  protected abstract IDictionary ServiceDictionary { get; }

  public IBusinessObjectService GetService (Type serviceType)
  {
    return (IBusinessObjectService) ServiceDictionary[serviceType];
  }

  public void AddService (Type serviceType, IBusinessObjectService service)
  {
    ServiceDictionary.Add (serviceType, service);
  }
}

}
