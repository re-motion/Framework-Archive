using System;

namespace Mixins.Definitions
{
  public class RequiredFaceTypeDefinition: RequirementDefinitionBase
  {
    public RequiredFaceTypeDefinition(BaseClassDefinition baseClass, Type type)
        : base(baseClass, type)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
