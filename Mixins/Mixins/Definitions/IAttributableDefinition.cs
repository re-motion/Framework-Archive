using System;

namespace Mixins.Definitions
{
  public interface IAttributableDefinition : IVisitableDefinition
  {
    MultiDefinitionItemCollection<Type, AttributeDefinition> CustomAttributes { get; }
  }
}
