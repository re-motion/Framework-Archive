using System;

namespace Mixins.Definitions
{
  public class ThisDependencyDefinition : DependencyDefinitionBase<RequiredFaceTypeDefinition, ThisDependencyDefinition>
  {
    public ThisDependencyDefinition (RequiredFaceTypeDefinition requiredType, MixinDefinition depender, ThisDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
