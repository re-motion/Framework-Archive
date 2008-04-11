using System;
using Remotion.Implementation;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  [ConcreteImplementation ("Remotion.Mixins.CodeGeneration.TypeFactoryImplementation, Remotion, Version = <version>")]
  public interface ITypeFactoryImplementation
  {
    Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy);
    TargetClassDefinition GetConfiguration (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy);
    ClassContext GetContext (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy);
    void InitializeUnconstructedInstance (IMixinTarget mixinTarget);
  }
}