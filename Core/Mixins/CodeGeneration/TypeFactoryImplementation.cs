using System;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  public class TypeFactoryImplementation : ITypeFactoryImplementation
  {
    public Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      TargetClassDefinition configuration = TypeFactory.GetActiveConfiguration (targetType, generationPolicy);
      if (configuration == null)
        return targetType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    public TargetClassDefinition GetConfiguration (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      context = TypeFactory.GetContext(targetType, mixinConfiguration, generationPolicy);

      if (context == null)
        return null;
      else
        return TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
    }

    public ClassContext GetContext (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      if (generationPolicy != GenerationPolicy.ForceGeneration && TypeUtility.IsGeneratedConcreteMixedType (targetType))
        context = TypeUtility.GetMixinConfigurationFromConcreteType (targetType);
      else
        context = mixinConfiguration.ClassContexts.GetWithInheritance (targetType);

      if (context == null && generationPolicy == GenerationPolicy.ForceGeneration)
        context = new ClassContext (targetType);

      if (context != null && targetType.IsGenericType && context.Type.IsGenericTypeDefinition)
        context = context.SpecializeWithTypeArguments (targetType.GetGenericArguments ());

      return context;
    }

    public void InitializeUnconstructedInstance (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ConcreteTypeBuilder.Current.InitializeUnconstructedInstance (mixinTarget);
    }
  }
}