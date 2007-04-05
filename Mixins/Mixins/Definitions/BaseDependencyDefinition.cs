using System;

namespace Mixins.Definitions
{
  public class BaseDependencyDefinition : DependencyDefinitionBase<RequiredBaseCallTypeDefinition>, IVisitableDefinition
  {
    public BaseDependencyDefinition (RequiredBaseCallTypeDefinition requiredType, MixinDefinition depender)
      : base (requiredType, depender)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
