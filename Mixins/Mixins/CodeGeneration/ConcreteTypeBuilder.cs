using System;
using Mixins.CodeGeneration.SingletonUtilities;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder : CallContextSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;

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

    public void ResetScope()
    {
      _scope = null;
    }

    // TODO: Add type caching to this class
    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ITypeGenerator generator = Scope.CreateTypeGenerator (configuration);
      Type finishedType = generator.GetBuiltType().CreateType();
      generator.InitializeStaticFields (finishedType);
      return finishedType;
    }

    // TODO: Add type caching to this class
    public Type GetConcreteMixinType (MixinDefinition configuration, Type[] genericArguments)
    {
      IMixinTypeGenerator generator = Scope.CreateMixinTypeGenerator (configuration, genericArguments);
      Type finishedType = generator.GetBuiltType ().CreateType ();
      return finishedType;
    }
  }
}
