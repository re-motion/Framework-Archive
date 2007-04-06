using System;

namespace Mixins.Definitions
{
  public class BaseDependencyDefinition : DependencyDefinitionBase<RequiredBaseCallTypeDefinition, BaseDependencyDefinition>
  {
    public BaseDependencyDefinition (RequiredBaseCallTypeDefinition requiredType, MixinDefinition depender, BaseDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
