using System;
using Mixins.CodeGeneration.SingletonUtilities;
using Mixins.Definitions;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder : CallContextSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;
    private InterlockedCache<BaseClassDefinition, Type> _typeCache = new InterlockedCache<BaseClassDefinition, Type>();

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

    // TODO: Add type caching to this class
    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (configuration, delegate (BaseClassDefinition classConfiguration)
      {
        ITypeGenerator generator = Scope.CreateTypeGenerator (classConfiguration);
        Type finishedType = generator.GetBuiltType ();
        generator.InitializeStaticFields (finishedType);
        return finishedType;
      });
    }

    // TODO: Add type caching to this class
    public Type GetConcreteMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      IMixinTypeGenerator generator = Scope.CreateMixinTypeGenerator (configuration);
      Type finishedType = generator.GetBuiltType ().CreateType ();
      return finishedType;
    }
  }
}
