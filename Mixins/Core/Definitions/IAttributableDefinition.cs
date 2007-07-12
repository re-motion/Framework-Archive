using System;

namespace Rubicon.Mixins.Definitions
{
  public interface IAttributableDefinition : IVisitableDefinition
  {
    MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes { get; }
  }
}
