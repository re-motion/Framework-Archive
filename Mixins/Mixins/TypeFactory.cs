using System;
using Mixins.CodeGeneration;
using Mixins.Utilities.Singleton;
using Mixins.Definitions;
using Mixins.Validation;
using Rubicon.Utilities;
using Mixins.Context;

namespace Mixins
{
  public static class TypeFactory
  {
    public static void InitializeMixedInstance (object instance)
    {
      if (instance is IMixinTarget)
        ConcreteTypeBuilder.Current.Scope.InitializeInstance (instance);
      else
        throw new ArgumentException ("The given instance does not implement IMixinTarget.", "instance");
    }

    public static void InitializeMixedInstanceWithMixins (object instance, object[] mixinInstances)
    {
      if (instance is IMixinTarget)
        ConcreteTypeBuilder.Current.Scope.InitializeInstanceWithMixins (instance, mixinInstances);
      else
        throw new ArgumentException ("The given instance does not implement IMixinTarget.", "instance");
    }

    // Instances of the type returned by this method must be initialized with <see cref="InitializeMixedInstance"/>
    public static Type GetConcreteType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      BaseClassDefinition configuration = GetActiveConfiguration (baseType);
      if (configuration == null)
        return baseType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    public static BaseClassDefinition GetActiveConfiguration (Type targetType)
    {
      ClassContext context = MixinConfiguration.ActiveContext.GetClassContext (targetType);
      if (context == null)
        context = new ClassContext (targetType);
      return BaseClassDefinitionCache.Current.GetBaseClassDefinition (context);
    }
  }
}