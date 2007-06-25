using System;

namespace Rubicon.Mixins.Definitions
{
  public interface IAttributableDefinition : IVisitableDefinition
  {
    MultiDefinitionItemCollection<Type, AttributeDefinition> CustomAttributes { get; }
  }
}
