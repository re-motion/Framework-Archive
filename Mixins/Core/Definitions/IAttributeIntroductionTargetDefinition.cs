using System;

namespace Rubicon.Mixins.Definitions
{
  public interface IAttributeIntroductionTargetDefinition : IAttributableDefinition, IVisitableDefinition
  {
    MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes { get; }
  }
}
