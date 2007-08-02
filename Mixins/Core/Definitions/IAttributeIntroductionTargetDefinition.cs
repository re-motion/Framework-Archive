using System;

namespace Rubicon.Mixins.Definitions
{
  public interface IAttributeIntroductionTargetDefinition : IAttributableDefinition
  {
    MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes { get; }
  }
}
