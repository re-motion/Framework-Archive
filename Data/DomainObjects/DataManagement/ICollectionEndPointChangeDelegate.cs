using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
internal interface ILinkChangeDelegate
{
  void PerformAdd (MultipleObjectsRelationLink link, DomainObject domainObject);
  void PerformRemove (MultipleObjectsRelationLink link, DomainObject domainObject);
}
}
