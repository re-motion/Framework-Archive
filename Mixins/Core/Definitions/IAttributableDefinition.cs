using System;

namespace Rubicon.Mixins.Definitions
{
  public interface IAttributableDefinition
  {
    MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes { get; }
  }
}
