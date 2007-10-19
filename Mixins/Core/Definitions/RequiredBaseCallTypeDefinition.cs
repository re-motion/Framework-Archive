using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Mixins.Definitions
{
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase
  {
    public RequiredBaseCallTypeDefinition (TargetClassDefinition targetClass, Type type)
        : base(targetClass, type)
    {
    }

    protected override void ConcreteAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
