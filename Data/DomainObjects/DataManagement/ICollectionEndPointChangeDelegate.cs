using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public interface ICollectionEndPointChangeDelegate
{
  void PerformAdd (CollectionEndPoint endPoint, DomainObject domainObject);
  void PerformInsert (CollectionEndPoint endPoint, DomainObject domainObject, int index);
  void PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject);
}
}
