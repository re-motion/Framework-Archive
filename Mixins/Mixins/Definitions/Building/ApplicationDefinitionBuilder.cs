using System;
using Mixins.Context;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class ApplicationDefinitionBuilder
  {
    private ApplicationDefinition _newApplication = new ApplicationDefinition ();
    private BaseClassDefinitionBuilder analyzer = new BaseClassDefinitionBuilder ();

    public void Apply (ApplicationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      foreach (ClassContext classContext in context.ClassContexts)
      {
        BaseClassDefinition classDefinition = analyzer.Apply (classContext);
        _newApplication.BaseClasses.Add (classDefinition);
      }
    }

    public ApplicationDefinition GetApplicationDefinition ()
    {
      return _newApplication;
    }
  }
}
