using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public interface ICollectionEndPointChangeDelegate
{
  void PerformAdd (CollectionEndPoint endPoint, DomainObject domainObject);
  void PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject);
}
}
