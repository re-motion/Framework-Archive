using System;

using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
{

public enum CardinalityType
{
  One = 0,
  Many = 1
}

public interface IRelationEndPointDefinition
{
  ClassDefinition ClassDefinition { get; }
  string PropertyName { get; }
  Type PropertyType { get; }
  bool IsMandatory { get; }
  CardinalityType Cardinality { get; }
  bool IsVirtual { get; }

  bool CorrespondsTo (string classID, string propertyName);
}
}
