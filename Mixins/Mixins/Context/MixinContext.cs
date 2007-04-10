using System;
using Rubicon.Utilities;

namespace Mixins.Context
{
  public struct MixinContext
  {
    public readonly Type TargetType;
    public readonly Type MixinType;

    public MixinContext (Type targetType, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      TargetType = targetType;
      MixinType = mixinType;
    }
  }
}
