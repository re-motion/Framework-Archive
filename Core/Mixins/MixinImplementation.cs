using System;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  public class MixinImplementation : IMixinImplementation
  {
    public TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get<TMixin> (mixinTarget);
    }

    public object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get (mixinType, mixinTarget);
    }

    public ClassContext GetMixinConfigurationFromConcreteType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ConcreteMixedTypeAttribute attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (type, true);
      if (attribute == null)
        return null;
      else
        return attribute.GetClassContext();
    }
  }
}