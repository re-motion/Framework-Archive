using System;
using Mixins.CodeGeneration;
using Mixins.Utilities.Singleton;
using Mixins.Definitions;
using Mixins.Validation;
using Rubicon.Utilities;

namespace Mixins
{
  public class TypeFactory : CallContextSingletonBase<TypeFactory, TypeFactory.TypeFactoryCreator>
  {
    public class TypeFactoryCreator : IInstanceCreator<TypeFactory>
    {
      public TypeFactory CreateInstance ()
      {
        return TypeFactory.DefaultInstance;
      }
    }

    public static readonly TypeFactory DefaultInstance = new TypeFactory ();

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

    public readonly ApplicationDefinition Configuration;

    // just for the default instance
    private TypeFactory()
    {
      Configuration = new ApplicationDefinition();
    }

    public TypeFactory (ApplicationDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      Configuration = configuration;

      Assertion.Assert (ValidateConfiguration(), "The configuration cannot be validated.");
    }

    private bool ValidateConfiguration()
    {
      DefaultValidationLog log = Validator.Validate (Configuration);
      if (log.GetNumberOfFailures() != 0 || log.GetNumberOfWarnings() != 0)
      {
        ConsoleDumper.DumpValidationResults (log.GetResults());
      }
      return log.GetNumberOfFailures() == 0;
    }

    // Instances of the type returned by this method must be initialized with <see cref="InitializeMixedInstance"/>
    public Type GetConcreteType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      BaseClassDefinition classConfig = Configuration.BaseClasses[baseType];
      if (classConfig != null)
      {
        return ConcreteTypeBuilder.Current.GetConcreteType (classConfig);
      }
      else
      {
        return baseType;
      }
    }
  }
}