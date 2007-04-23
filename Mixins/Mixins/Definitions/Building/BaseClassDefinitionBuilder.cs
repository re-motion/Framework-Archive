using System;
using System.Collections.Generic;
using Mixins.Context;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class BaseClassDefinitionBuilder
  {
    private ApplicationDefinition _application;

    public BaseClassDefinitionBuilder (ApplicationDefinition application)
    {
      ArgumentUtility.CheckNotNull ("application", application);
      _application = application;
    }

    public ApplicationDefinition Application
    {
      get { return _application; }
    }

    public void Apply (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      BaseClassDefinition classDefinition = new BaseClassDefinition (_application, classContext.Type);
      Application.BaseClasses.Add (classDefinition);

      ClassDefinitionBuilderHelper.InitializeMembers (classDefinition, IsVisibleToInheritors);
      ApplyMixins(classDefinition, classContext);

    }

    private static bool IsVisibleToInheritors (MethodInfo method)
    {
      return method.IsPublic || method.IsFamily;
    }

    private static void ApplyMixins (BaseClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      MixinDefinitionBuilder mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      IEnumerator<MixinContext> enumerator = classContext.MixinContexts.GetEnumerator();
      for (int i = 0; enumerator.MoveNext(); ++i)
      {
        MixinDefinition mixin = mixinDefinitionBuilder.Apply (enumerator.Current);
        mixin.MixinIndex = i;
      }
    }
  }
}
