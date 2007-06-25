using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  public static class UnvalidatedDefinitionBuilder
  {
    public static BaseClassDefinition BuildUnvalidatedDefinition (Type baseType, params Type[] mixinTypes)
    {
      ClassContext context = new ClassContext (baseType, mixinTypes);
      BaseClassDefinitionBuilder builder = new BaseClassDefinitionBuilder();
      return builder.Build (context);
    }
  }
}
