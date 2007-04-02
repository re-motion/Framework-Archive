using System;
using Mixins.Context;
using System.Reflection;

namespace Mixins.Definitions.Building
{
  public class BaseClassDefinitionBuilder
  {
    private ApplicationDefinition _application;

    public BaseClassDefinitionBuilder (ApplicationDefinition application)
    {
      _application = application;
    }

    public ApplicationDefinition Application
    {
      get { return _application; }
    }

    public void Apply (ClassContext classContext)
    {
      BaseClassDefinition classDefinition = new BaseClassDefinition (classContext.Type);
      Application.AddBaseClass (classDefinition);

      InitializeMembers (classDefinition);
      ApplyMixins(classDefinition, classContext);

    }

    private void InitializeMembers (BaseClassDefinition classDefinition)
    {
      foreach (MethodInfo method in classDefinition.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic || method.IsFamily)
        {
          classDefinition.AddMember (new MethodDefinition (method, classDefinition));
        }
      }
    }

    private static void ApplyMixins(BaseClassDefinition classDefinition, ClassContext classContext)
    {
      MixinDefinitionBuilder mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      foreach (MixinContext mixinContext in classContext.MixinContexts)
      {
        mixinDefinitionBuilder.Apply (mixinContext);
      }
    }
  }
}
