using System;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class BaseDependencyDefinition : DependencyDefinitionBase<RequiredBaseCallTypeDefinition, BaseDependencyDefinition>
  {
    public BaseDependencyDefinition (RequiredBaseCallTypeDefinition requiredType, MixinDefinition depender, BaseDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
