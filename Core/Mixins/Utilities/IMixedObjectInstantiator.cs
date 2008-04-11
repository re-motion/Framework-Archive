using System;
using Remotion.Implementation;
using Remotion.Reflection;

namespace Remotion.Mixins.Utilities
{
  [ConcreteImplementation("Remotion.Mixins.Utilities.MixedObjectInstantiator, Remotion, Version = <version>")]
  public interface IMixedObjectInstantiator
  {
    FuncInvokerWrapper<T> CreateConstructorInvoker<T> (Type baseTypeOrInterface, GenerationPolicy generationPolicy, bool allowNonPublic,
        params object[] preparedMixins);
  }
}