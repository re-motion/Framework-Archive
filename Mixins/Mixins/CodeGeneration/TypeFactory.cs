using System;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public class TypeFactory
  {
    private ApplicationDefinition _configuration;
    private ConcreteTypeBuilder _typeBuilder;

    public TypeFactory(ApplicationDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      _configuration = configuration;
      _typeBuilder = new ConcreteTypeBuilder(new DynamicProxy.ModuleManager());
    }

    public Type GetConcreteType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      BaseClassDefinition classConfig = _configuration.BaseClasses[baseType];
      if (classConfig != null)
      {
        return _typeBuilder.GetConcreteType (classConfig);
      }
      else
      {
        return baseType;
      }
    }

    public static void InitializeInstance (object instance)
    {
      if (instance is IMixinTarget)
      {
        GeneratedClassInstanceInitializer.InitializeInstanceFields (instance);
      }
    }

    public ConcreteTypeBuilder TypeBuilder
    {
      get { return _typeBuilder; }
    }
  }
}
