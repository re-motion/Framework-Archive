using System;
using Mixins.Context;
using Mixins.Definitions.Building;

namespace Mixins.Definitions.Building
{
  public static class DefinitionBuilder
  {
    public static ApplicationDefinition CreateApplicationDefinition (ApplicationContext context)
    {
      ApplicationDefinitionBuilder builder = new ApplicationDefinitionBuilder ();
      builder.Apply (context);
      return builder.GetApplicationDefinition ();
    }

    public static BaseClassDefinition GetMergedBaseClassDefinition (Type type, ApplicationDefinition source)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
