using System;

namespace Mixins.Definitions
{
  public class ThisDependencyDefinition : DependencyDefinitionBase<RequiredFaceTypeDefinition>, IVisitableDefinition
  {
    public ThisDependencyDefinition (RequiredFaceTypeDefinition requiredType, MixinDefinition depender)
      : base (requiredType, depender)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
