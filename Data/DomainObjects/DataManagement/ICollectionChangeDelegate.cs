using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
internal interface ICollectionChangeDelegate
{
  void PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index);
  void PerformReplace (DomainObjectCollection collection, DomainObject domainObject, int index);
  void PerformRemove (DomainObjectCollection collection, DomainObject domainObject);
}
}
