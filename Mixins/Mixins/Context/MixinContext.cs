using System;

namespace Mixins.Context
{
  public struct MixinContext
  {
    public readonly Type TargetType;
    public readonly Type MixinType;

    public MixinContext (Type targetType, Type mixinType)
    {
      TargetType = targetType;
      MixinType = mixinType;
    }
  }
}
