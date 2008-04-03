using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;

namespace Remotion.Mixins.UnitTests
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
