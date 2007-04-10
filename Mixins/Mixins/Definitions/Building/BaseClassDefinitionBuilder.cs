using System;
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

      InitializeMembers (classDefinition);
      ApplyMixins(classDefinition, classContext);

    }

    private void InitializeMembers (BaseClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      foreach (MethodInfo method in classDefinition.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic || method.IsFamily)
        {
          classDefinition.Members.Add (new MethodDefinition (method, classDefinition));
        }
      }
    }

    private static void ApplyMixins(BaseClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      MixinDefinitionBuilder mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      foreach (MixinContext mixinContext in classContext.MixinContexts)
      {
        mixinDefinitionBuilder.Apply (mixinContext);
      }
    }
  }
}
