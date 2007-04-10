using System;
using Mixins.Context;
using Mixins.Definitions.Building;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public static class DefinitionBuilder
  {
    public static ApplicationDefinition CreateApplicationDefinition (ApplicationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      ApplicationDefinitionBuilder builder = new ApplicationDefinitionBuilder ();
      builder.Apply (context);
      return builder.GetApplicationDefinition ();
    }
  }
}
