using System;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public interface IEndPoint : INullObject
{
  ClientTransaction ClientTransaction { get; }
  DomainObject GetDomainObject ();
  DataContainer GetDataContainer ();
  ObjectID ObjectID { get; }

  RelationDefinition RelationDefinition { get; }
  IRelationEndPointDefinition Definition { get; }
}
}
