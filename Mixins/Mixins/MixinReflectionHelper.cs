using System;
using Mixins.Definitions;

namespace Mixins
{
  public static class MixinReflectionHelper
  {
    public static T GetMixinOf<T> (object mixinTarget) where T : class
    {
      IMixinTarget castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        MixinDefinition mixinDefinition = castMixinTarget.Configuration.Mixins[typeof (T)];
        if (mixinDefinition != null)
        {
          return (T) castMixinTarget.Mixins[mixinDefinition.MixinIndex];
        }
      }
      return null;
    }
  }
}
