using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase
  {
    public RequiredBaseCallTypeDefinition (BaseClassDefinition baseClass, Type type)
        : base(baseClass, type)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
