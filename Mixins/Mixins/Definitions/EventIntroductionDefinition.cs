using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class EventIntroductionDefinition : MemberIntroductionDefinition<EventInfo, EventDefinition>
  {
    public EventIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, EventInfo interfaceMember, EventDefinition implementingMember)
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
