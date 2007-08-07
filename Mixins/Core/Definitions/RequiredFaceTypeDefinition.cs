using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  public class RequiredFaceTypeDefinition : RequirementDefinitionBase
  {
    public RequiredFaceTypeDefinition (TargetClassDefinition targetClass, Type type)
      : base (targetClass, type)
    {
    }

    protected override void ConcreteAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
