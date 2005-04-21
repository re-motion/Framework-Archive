using System;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public interface IEndPoint : INullableObject
{
  ClientTransaction ClientTransaction { get; }
  DomainObject GetDomainObject ();
  DataContainer GetDataContainer ();
  ObjectID ObjectID { get; }

  ClassDefinition ClassDefinition { get; }
  RelationDefinition RelationDefinition { get; }
  IRelationEndPointDefinition Definition { get; }
  
}
}
