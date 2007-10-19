using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  public class MethodIntroductionDefinition : MemberIntroductionDefinition<MethodInfo, MethodDefinition>
  {
    public MethodIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, MethodInfo interfaceMember, MethodDefinition implementingMember)
        : base (declaringInterface, interfaceMember, implementingMember)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
