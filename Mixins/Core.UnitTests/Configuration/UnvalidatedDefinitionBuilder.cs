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
    public static TargetClassDefinition BuildUnvalidatedDefinition (Type baseType, params Type[] mixinTypes)
    {
      ClassContext context = new ClassContext (baseType, mixinTypes);
      return BuildUnvalidatedDefinition(context);
    }

    public static TargetClassDefinition BuildUnvalidatedDefinition (ClassContext context)
    {
      TargetClassDefinitionBuilder builder = new TargetClassDefinitionBuilder();
      return builder.Build (context);
    }
  }
}
