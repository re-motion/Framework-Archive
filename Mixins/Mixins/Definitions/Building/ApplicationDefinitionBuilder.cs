using System;
using Mixins.Context;

namespace Mixins.Definitions.Building
{
  public class ApplicationDefinitionBuilder
  {
    private ApplicationDefinition _newApplication = new ApplicationDefinition ();

    public void Apply (ApplicationContext context)
    {
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
