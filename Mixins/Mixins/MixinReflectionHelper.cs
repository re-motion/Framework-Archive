using System;
using Mixins.Definitions;
using System.Reflection;

namespace Mixins
{
  public static class MixinReflectionHelper
  {
    public static T GetMixinOf<T> (object mixinTarget) where T : class
    {
      return (T) GetMixinOf (typeof (T), mixinTarget);
    }

    public static object GetMixinOf (Type mixinType, object mixinTarget)
    {
      IMixinTarget castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        MixinDefinition mixinDefinition = castMixinTarget.Configuration.Mixins[mixinType];
        if (mixinDefinition != null)
        {
          return castMixinTarget.Mixins[mixinDefinition.MixinIndex];
        }
      }
      return null;
    }
  }
}
