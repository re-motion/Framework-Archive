using System;
using Rubicon.Utilities;

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
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
