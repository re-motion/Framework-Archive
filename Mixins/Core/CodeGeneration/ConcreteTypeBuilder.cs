using System;
using Rubicon.Mixins.Utilities.Singleton;
using Rubicon.Mixins.Definitions;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder : CallContextSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;
    // No laziness here - a ModuleBuilder cannot be used by multiple threads at the same time anyway, so using a lazy cache could actually cause
    // errors (depending on how it was implemented)
    private InterlockedCache<ClassDefinition, Type> _typeCache = new InterlockedCache<ClassDefinition, Type>();

		private object _scopeLockObject = new object ();

    public IModuleManager Scope
    {
      get
      {
				lock (_scopeLockObject)
				{
					if (_scope == null)
					{
						_scope = new DynamicProxy.ModuleManager();
					}
					return _scope;
				}
      }
      set
      {
				ArgumentUtility.CheckNotNull ("value", value);
				lock (_scopeLockObject)
				{
					_scope = value;
				}
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
