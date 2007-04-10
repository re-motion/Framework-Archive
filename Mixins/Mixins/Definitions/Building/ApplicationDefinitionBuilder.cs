using System;
using Mixins.Context;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class ApplicationDefinitionBuilder
  {
    private ApplicationDefinition _newApplication = new ApplicationDefinition ();

    public void Apply (ApplicationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      BaseClassDefinitionBuilder analyzer = new BaseClassDefinitionBuilder (_newApplication);

      foreach (ClassContext classContext in context.ClassContexts)
      {
        analyzer.Apply (classContext);
      }
    }

    public ApplicationDefinition GetApplicationDefinition ()
    {
      return _newApplication;
    }
  }
}
