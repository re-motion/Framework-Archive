using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using System.Reflection;

namespace Mixins.Definitions
{
  [Serializable]
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase<RequiredBaseCallTypeDefinition, BaseDependencyDefinition>
  {
    public readonly DefinitionItemCollection<MemberInfo, RequiredBaseCallMethodDefinition> BaseCallMethods =
        new DefinitionItemCollection<MemberInfo, RequiredBaseCallMethodDefinition> (delegate (RequiredBaseCallMethodDefinition m)
        { return m.InterfaceMethod; });

    public RequiredBaseCallTypeDefinition (BaseClassDefinition baseClass, Type type)
        : base(baseClass, type)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      BaseCallMethods.Accept (visitor);
    }
  }
}
