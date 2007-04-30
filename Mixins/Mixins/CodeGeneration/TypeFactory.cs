using System;
using Mixins.Definitions;
using Mixins.Validation.DefaultLog;
using Rubicon.Utilities;
using Mixins.Validation;

namespace Mixins.CodeGeneration
{
  public class TypeFactory
  {
    public static void InitializeInstance (object instance)
    {
      if (instance is IMixinTarget)
      {
        ConcreteTypeBuilder.Scope.Current.InitializeInstance (instance);
      }
    }

    private ApplicationDefinition _configuration;

    public TypeFactory(ApplicationDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      _configuration = configuration;

      Assertion.DebugAssert (ValidateConfiguration ());
    }

    private bool ValidateConfiguration ()
    {
      DefaultValidationLog log = Validator.Validate (_configuration);
      if (log.GetNumberOfFailureResults () != 0)
      {
        ConsoleDumper.DumpLog (log);
        return false;
      }
      else if (log.GetNumberOfWarningResults () != 0)
      {
        ConsoleDumper.DumpLog (log);
        return true;
      }
      else
      {
        return true;
      }
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
