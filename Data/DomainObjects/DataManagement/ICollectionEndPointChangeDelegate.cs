using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
internal interface ICollectionEndPointChangeDelegate
{
  void PerformAdd (CollectionEndPoint endPoint, DomainObject domainObject);
  void PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject);
}
}
