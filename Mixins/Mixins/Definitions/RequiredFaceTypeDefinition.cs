using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase<RequiredBaseCallTypeDefinition, BaseDependencyDefinition>
  {
    public RequiredBaseCallTypeDefinition (BaseClassDefinition baseClass, Type type)
        : base(baseClass, type)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
