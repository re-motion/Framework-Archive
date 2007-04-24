using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class PropertyIntroductionDefinition : MemberIntroductionDefinition<PropertyInfo, PropertyDefinition>
  {
    public PropertyIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, PropertyInfo interfaceMember, PropertyDefinition implementingMember)
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
