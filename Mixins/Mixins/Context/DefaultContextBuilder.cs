using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Context
{
  public static class DefaultContextBuilder
  {
    public static ApplicationContext BuildContextFromAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      ApplicationContext context = new ApplicationContext ();
      AnalyzeAssemblyIntoContext (assembly, context);
      return context;
    }

    public static ApplicationContext BuildContextFromAssemblies (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      ApplicationContext context = new ApplicationContext ();
      foreach (Assembly assembly in assemblies)
      {
        AnalyzeAssemblyIntoContext (assembly, context);
      }
      return context;
    }

    public static void AnalyzeAssemblyIntoContext (Assembly assembly, ApplicationContext targetContext)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("targetContext", targetContext);

      foreach (Type t in assembly.GetTypes ())
      {
        if (t.IsDefined (typeof (ExtendsAttribute), false))
          AnalyzeMixin (t, targetContext);
        if (t.IsDefined (typeof (UsesAttribute), true))
          AnalyzeMixinApplications (t, targetContext);
      }
    }

    private static void AnalyzeMixin(Type mixinType, ApplicationContext targetContext)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("targetContext", targetContext);

      foreach (ExtendsAttribute mixinAttribute in mixinType.GetCustomAttributes(typeof(ExtendsAttribute), false))
      {
        targetContext.GetOrAddClassContext (mixinAttribute.TargetType).AddMixin (mixinType);
      }
    }

    private static void AnalyzeMixinApplications(Type targetType, ApplicationContext targetContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("targetContext", targetContext);

      foreach (UsesAttribute applyMixinAttribute in targetType.GetCustomAttributes (typeof (UsesAttribute), true))
      {
        targetContext.GetOrAddClassContext (targetType).AddMixin (applyMixinAttribute.MixinType);
      }
    }

    public static ApplicationContext BuildDefaultContext ()
    {
      return new ApplicationContext();
    }
  }
}
