using System;
using Remotion.Implementation;
using Remotion.Mixins.Context;

namespace Remotion.Mixins
{
  [ConcreteImplementation ("Remotion.Mixins.Utilities.MixinImplementation, Remotion, Version = <version>")]
  public interface IMixinImplementation
  {
    TMixin Get<TMixin> (object mixinTarget) where TMixin : class;
    object Get (Type mixinType, object mixinTarget);
    ClassContext GetMixinConfigurationFromConcreteType (Type type);
  }
}