using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
internal interface ICollectionChangeDelegate
{
  void PerformAdd (DomainObjectCollection collection, DomainObject domainObject);
  void PerformRemove (DomainObjectCollection collection, DomainObject domainObject);
}
}
