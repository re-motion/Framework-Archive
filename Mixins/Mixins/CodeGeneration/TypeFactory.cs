using System;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public class TypeFactory
  {
    public static void InitializeInstance (object instance)
    {
      if (instance is IMixinTarget)
      {
        GeneratedClassInstanceInitializer.InitializeInstanceFields (instance);
      }
    }

    private ApplicationDefinition _configuration;

    public TypeFactory(ApplicationDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      _configuration = configuration;
    }

    // Instances returned by this method must be initialized with <see cref="InitializeInstance"/>
    public Type GetConcreteType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      BaseClassDefinition classConfig = _configuration.BaseClasses[baseType];
      if (classConfig != null)
      {
        return ConcreteTypeBuilder.Instance.Current.GetConcreteType (classConfig);
      }
      else
      {
        return baseType;
      }
    }
  }
}
