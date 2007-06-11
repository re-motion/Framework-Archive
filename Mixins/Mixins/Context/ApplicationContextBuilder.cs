using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mixins;
using Rubicon.Utilities;
using Rubicon.Reflection;

namespace Mixins.Context
{
  public static class ApplicationContextBuilder
  {
    public static ApplicationContext BuildFromAssemblies (ApplicationContext parentContext, IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      // AnalyzeAssemblyIntoContext will not overwrite any existing class contexts, but instead augment them with any new definitions.
      // This is exactly what we want for the assemblies, since all of these are of equal priority. However, we want to replace those contexts
      // inherited from the parent context.

      // Therefore, first analyze the assemblies into a temporary context without replacements:
      ApplicationContext tempContext = new ApplicationContext();
      foreach (Assembly assembly in assemblies)
        AnalyzeAssemblyIntoContext (assembly, tempContext);

      // Then, add them to the resulting context, replacing the respective inherited class contexts:
      ApplicationContext fullContext = new ApplicationContext (parentContext);
      foreach (ClassContext classContext in tempContext.ClassContexts)
        fullContext.AddOrReplaceClassContext (classContext);
      return fullContext;
    }

    public static ApplicationContext BuildFromAssemblies (ApplicationContext parentContext, params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildFromAssemblies (parentContext, (IEnumerable<Assembly>) assemblies);
    }

    public static ApplicationContext BuildFromAssemblies (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildFromAssemblies (null, (IEnumerable<Assembly>) assemblies);
    }

    public static void AnalyzeAssemblyIntoContext (Assembly assembly, ApplicationContext targetContext)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("targetContext", targetContext);

      foreach (Type t in assembly.GetTypes())
      {
        if (t.IsDefined (typeof (ExtendsAttribute), false))
          AnalyzeMixin (t, targetContext);
        if (t.IsDefined (typeof (UsesAttribute), true))
          AnalyzeMixinApplications (t, targetContext);
        if (t.IsDefined (typeof (CompleteInterfaceAttribute), false))
          AnalyzeCompleteInterface (t, targetContext);
      }
    }

    private static void AnalyzeMixin (Type mixinType, ApplicationContext targetContext)
    {
      foreach (ExtendsAttribute mixinAttribute in mixinType.GetCustomAttributes (typeof (ExtendsAttribute), false))
        targetContext.GetOrAddClassContext (mixinAttribute.TargetType).AddMixin (mixinType);
    }

    private static void AnalyzeMixinApplications (Type targetType, ApplicationContext targetContext)
    {
      foreach (UsesAttribute applyMixinAttribute in targetType.GetCustomAttributes (typeof (UsesAttribute), true))
        targetContext.GetOrAddClassContext (targetType).AddMixin (applyMixinAttribute.MixinType);
    }

    private static void AnalyzeCompleteInterface (Type completeInterfaceType, ApplicationContext targetContext)
    {
      foreach (CompleteInterfaceAttribute ifaceAttribute in completeInterfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
        targetContext.GetOrAddClassContext (ifaceAttribute.TargetType).AddCompleteInterface (completeInterfaceType);
    }

    /// <summary>
    /// Builds the default application context by analyzing all currently loaded assemblies and their (directly or indirectly) referenced assemblies
    /// for mixin configuration information if they are marked with the <see cref="ContainsMixinInfoAttribute"/>.
    /// </summary>
    /// <returns>An application context holding the default mixin configuration information for this application.</returns>
    /// <remarks>This method performs the following steps (see also <see cref="AssemblyFinder"/>):
    /// <list type="number">
    /// <item>Retrieve all assemblies loaded into the current <see cref="AppDomain"/>.</item>
    /// <item>Analyze each of them marked with the <see cref="ContainsMixinInfoAttribute"/> for mixin configuration information.</item>
    /// <item>Load the referenced assemblies of those assemblies marked with the attribute.</item>
    /// <item>If the loaded assemblies haven't already been analyzed, treat them according to steps 2-4.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="AssemblyFinder"/>
    public static ApplicationContext BuildDefault ()
    {
      Assembly[] rootAssemblies =
          Array.FindAll (
              AppDomain.CurrentDomain.GetAssemblies(), delegate (Assembly a) { return a.IsDefined (typeof (ContainsMixinInfoAttribute), false); });

      Assembly[] assembliesToBeScanned = rootAssemblies;
      if (rootAssemblies.Length > 0)
      {
        AssemblyFinder finder = new AssemblyFinder (typeof (ContainsMixinInfoAttribute), rootAssemblies);
        assembliesToBeScanned = finder.FindAssemblies();
      }
      return BuildFromAssemblies (assembliesToBeScanned);
    }

    public static ApplicationContext BuildFromClasses (ApplicationContext parentContext, params ClassContext[] classContexts)
    {
      ArgumentUtility.CheckNotNull ("classContexts", classContexts);

      ApplicationContext context = new ApplicationContext (parentContext);
      foreach (ClassContext classContext in classContexts)
        context.AddOrReplaceClassContext (classContext);
      return context;
    }
  }
}
