using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;

namespace Mixins.UnitTests.Configuration
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
