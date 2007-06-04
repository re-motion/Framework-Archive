using System;
using Mixins.Utilities.Singleton;
using Mixins.Definitions;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder : CallContextSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;
    // No laziness here - a ModuleBuilder cannot be used by multiple threads at the same time anyway, so using a lazy cache would actually cause
    // errors
    private InterlockedCache<ClassDefinition, Type> _typeCache = new InterlockedCache<ClassDefinition, Type>();

    public IModuleManager Scope
    {
      get
      {
        if (_scope == null)
        {
          _scope = new DynamicProxy.ModuleManager();
        }
        return _scope;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _scope = value;
      }
    }

    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (configuration, delegate (ClassDefinition classConfiguration)
      {
        ITypeGenerator generator = Scope.CreateTypeGenerator ((BaseClassDefinition) classConfiguration);
        Type finishedType = generator.GetBuiltType ();
        return finishedType;
      });
    }

    // TODO: Add type caching to this class
    public Type GetConcreteMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (configuration, delegate (ClassDefinition classConfiguration)
      {
        IMixinTypeGenerator generator = Scope.CreateMixinTypeGenerator ((MixinDefinition) classConfiguration);
        Type finishedType = generator.GetBuiltType ();
        return finishedType;
      });
    }
  }
}
