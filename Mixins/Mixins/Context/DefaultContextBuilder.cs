using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mixins.Context
{
  public static class DefaultContextBuilder
  {
    public static ApplicationContext BuildContextFromAssembly (Assembly assembly)
    {
      ApplicationContext context = new ApplicationContext ();
      AnalyzeAssemblyIntoContext (assembly, context);
      return context;
    }

    public static ApplicationContext BuildContextFromAssemblies (IEnumerable<Assembly> assemblies)
    {
      ApplicationContext context = new ApplicationContext ();
      foreach (Assembly assembly in assemblies)
      {
        AnalyzeAssemblyIntoContext (assembly, context);
      }
      return context;
    }

    public static void AnalyzeAssemblyIntoContext (Assembly assembly, ApplicationContext targetContext)
    {
      foreach (Type t in assembly.GetTypes())
      {
        if (t.IsDefined (typeof (MixinAttribute), true))
        {
          foreach (MixinAttribute mixinAttribute in t.GetCustomAttributes(typeof(MixinAttribute), false))
          {
            MixinDefinition definition = new MixinDefinition(mixinAttribute, t);
            targetContext.GetOrAddClassContext (definition.TargetType).AddMixinDefinition (definition);
          }
        }
      }
    }
  }
}
