using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  public class RequiredFaceTypeDefinition : RequirementDefinitionBase<RequiredFaceTypeDefinition, ThisDependencyDefinition>
  {
    public RequiredFaceTypeDefinition (BaseClassDefinition baseClass, Type type)
      : base (baseClass, type)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
