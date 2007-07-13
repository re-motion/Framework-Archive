using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase
  {
    public RequiredBaseCallTypeDefinition (BaseClassDefinition baseClass, Type type)
        : base(baseClass, type)
    {
    }

    protected override void ConcreteAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
