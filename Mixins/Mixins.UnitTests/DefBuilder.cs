using System;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests
{
  public static class DefBuilder
  {
    public static ApplicationDefinition Build (Type classType, params Type[] mixinTypes)
    {
      ApplicationContext applicationContext = new ApplicationContext ();
      ClassContext classContext = applicationContext.GetOrAddClassContext (classType);
      foreach (Type mixinType in mixinTypes)
      {
        classContext.AddMixinContext (new MixinContext (classType, mixinType));
      }
      return DefinitionBuilder.CreateApplicationDefinition (applicationContext);
    }
  }
}
