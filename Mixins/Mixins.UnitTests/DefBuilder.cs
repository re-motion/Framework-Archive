using System;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Mixins.UnitTests
{
  public static class DefBuilder
  {
    public static ApplicationDefinition Build ()
    {
      ApplicationContext applicationContext = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly());
      return DefinitionBuilder.CreateApplicationDefinition (applicationContext);
    }


    public static ApplicationDefinition Build (Type classType, params Type[] mixinTypes)
    {
      ApplicationContext applicationContext = new ApplicationContext ();
      ClassContext classContext = applicationContext.GetOrAddClassContext (classType);
      foreach (Type mixinType in mixinTypes)
      {
        classContext.AddMixin (mixinType);
      }
      return DefinitionBuilder.CreateApplicationDefinition (applicationContext);
    }
  }
}
