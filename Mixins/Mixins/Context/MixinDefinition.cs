using System;

namespace Mixins.Context
{
  public struct MixinDefinition
  {
    public readonly MixinAttribute DefiningAttribute;
    public readonly Type MixinType;

    public MixinDefinition (MixinAttribute definingAttribute, Type mixinType)
    {
      DefiningAttribute = definingAttribute;
      MixinType = mixinType;
    }

    public Type TargetType
    {
      get { return DefiningAttribute.TargetType; }
    }
  }
}
