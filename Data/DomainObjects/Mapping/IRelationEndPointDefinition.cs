using System;

using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Mapping
{
public interface IRelationEndPointDefinition : INullableObject
{
  RelationDefinition RelationDefinition { get; }
  ClassDefinition ClassDefinition { get; }
  string PropertyName { get; }
  Type PropertyType { get; }
  bool IsMandatory { get; }
  CardinalityType Cardinality { get; }
  bool IsVirtual { get; }

  bool CorrespondsTo (string classID, string propertyName);
  void SetRelationDefinition (RelationDefinition relationDefinition);
}
}
