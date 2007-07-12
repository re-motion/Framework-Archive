using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
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

    public override ClassDefinitionBase GetImplementer ()
    {
      ClassDefinitionBase implementer = base.GetImplementer ();
      // check for duck interface
      if (implementer == null && !RequiredType.IsEmptyInterface)
      {
        implementer = Depender.BaseClass; // duck interface
      }
      return implementer;
    }
  }
}
