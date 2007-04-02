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
        if (t.IsDefined (typeof (MixinForAttribute), false))
        {
          AnalyzeMixin(t, targetContext);
        }
        if (t.IsDefined (typeof (ApplyMixinAttribute), true))
        {
          AnalyzeMixinApplications(t, targetContext);
        }
      }
    }

    private static void AnalyzeMixin(Type mixinType, ApplicationContext targetContext)
    {
      foreach (MixinForAttribute mixinAttribute in mixinType.GetCustomAttributes(typeof(MixinForAttribute), false))
      {
        MixinContext definition = new MixinContext(mixinAttribute.TargetType, mixinType);
        targetContext.GetOrAddClassContext (definition.TargetType).AddMixinContext (definition);
      }
    }

    private static void AnalyzeMixinApplications(Type targetType, ApplicationContext targetContext)
    {
      foreach (ApplyMixinAttribute applyMixinAttribute in targetType.GetCustomAttributes (typeof (ApplyMixinAttribute), true))
      {
        MixinContext definition = new MixinContext (targetType, applyMixinAttribute.MixinType);
        targetContext.GetOrAddClassContext (targetType).AddMixinContext (definition);
      }
    }
  }
}
