using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mixins;
using Rubicon.Collections;
using Rubicon.Utilities;
using Rubicon.Reflection;
using Mixins.Definitions.Building;

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
        if (!t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
          AnalyzeTypeIntoContext (t, targetContext);
      }
    }

    public static void AnalyzeTypeIntoContext (Type type, ApplicationContext targetContext)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("targetContext", targetContext);

      if (type.IsDefined (typeof (ExtendsAttribute), false))
        AnalyzeMixin (type, targetContext);
      if (type.IsDefined (typeof (UsesAttribute), true))
        AnalyzeMixinApplications (type, targetContext);
      if (type.IsDefined (typeof (CompleteInterfaceAttribute), false))
        AnalyzeCompleteInterface (type, targetContext);
    }

    private static void AnalyzeMixin (Type mixinType, ApplicationContext targetContext)
    {
      foreach (ExtendsAttribute mixinAttribute in mixinType.GetCustomAttributes (typeof (ExtendsAttribute), false))
        targetContext.GetOrAddClassContext (mixinAttribute.TargetType).AddMixin (mixinType);
    }

    private static void AnalyzeMixinApplications (Type targetType, ApplicationContext targetContext)
    {
      ClassContext classContext = targetContext.GetOrAddClassContext (targetType);
      Type currentType = targetType;
      while (currentType != null)
      {
        UsesAttribute[] attributes = (UsesAttribute[]) currentType.GetCustomAttributes (typeof (UsesAttribute), false);
        EnsureNoUsesDuplicates (attributes, targetType);
        foreach (UsesAttribute usesAttribute in attributes)
          ApplyMixinToClassContext (classContext, usesAttribute.MixinType, usesAttribute.AdditionalDependencies);
        currentType = currentType.BaseType;
      }
    }

    private static void EnsureNoUsesDuplicates (UsesAttribute[] usesAttributes, Type targetType)
    {
      Set<Type> mixinTypes = new Set<Type> ();
      foreach (UsesAttribute usesAttribute in usesAttributes)
      {
        if (mixinTypes.Contains (usesAttribute.MixinType))
        {
          string message = string.Format ("Two instances of mixin {0} are configured for target type {1}.",
              usesAttribute.MixinType.FullName, targetType.FullName);
          throw new ConfigurationException (message);
        }
        else
          mixinTypes.Add (usesAttribute.MixinType);
      }
    }

    private static void ApplyMixinToClassContext (ClassContext classContext, Type mixinType, IEnumerable<Type> explicitDependencies)
    {
      if (!classContext.ContainsMixin (mixinType))
      {
        MixinContext mixinContext = classContext.GetOrAddMixinContext (mixinType);
        foreach (Type additionalDependency in explicitDependencies)
          mixinContext.AddExplicitDependency (additionalDependency);
      }
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
