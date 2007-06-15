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
  public class ApplicationContextBuilder
  {
    public static ApplicationContext BuildContextFromAssemblies (ApplicationContext parentContext, IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      // AnalyzeAssemblyIntoContext will not overwrite any existing class contexts, but instead augment them with any new definitions.
      // This is exactly what we want for the assemblies, since all of these are of equal priority. However, we want to replace those contexts
      // inherited from the parent context.

      // Therefore, first analyze the assemblies into a temporary context without replacements:
      ApplicationContextBuilder tempContextBuilder = new ApplicationContextBuilder (null);
      foreach (Assembly assembly in assemblies)
        tempContextBuilder.AddAssembly (assembly);

      ApplicationContext tempContext = tempContextBuilder.Analyze ();

      // Then, add them to the resulting context, replacing the respective inherited class contexts:
      ApplicationContext fullContext = new ApplicationContext (parentContext);
      foreach (ClassContext classContext in tempContext.ClassContexts)
        fullContext.AddOrReplaceClassContext (classContext);
      return fullContext;
    }

    public static ApplicationContext BuildContextFromAssemblies (ApplicationContext parentContext, params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildContextFromAssemblies (parentContext, (IEnumerable<Assembly>) assemblies);
    }

    public static ApplicationContext BuildContextFromAssemblies (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildContextFromAssemblies (null, (IEnumerable<Assembly>) assemblies);
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
    public static ApplicationContext BuildDefaultContext ()
    {
      Assembly[] rootAssemblies =
          Array.FindAll (
              AppDomain.CurrentDomain.GetAssemblies (), delegate (Assembly a) { return a.IsDefined (typeof (ContainsMixinInfoAttribute), false); });

      Assembly[] assembliesToBeScanned = rootAssemblies;
      if (rootAssemblies.Length > 0)
      {
        AssemblyFinder finder = new AssemblyFinder (typeof (ContainsMixinInfoAttribute), rootAssemblies);
        assembliesToBeScanned = finder.FindAssemblies ();
      }
      return BuildContextFromAssemblies (assembliesToBeScanned);
    }

    public static ApplicationContext BuildContextFromClasses (ApplicationContext parentContext, params ClassContext[] classContexts)
    {
      ArgumentUtility.CheckNotNull ("classContexts", classContexts);

      ApplicationContext context = new ApplicationContext (parentContext);
      foreach (ClassContext classContext in classContexts)
        context.AddOrReplaceClassContext (classContext);
      return context;
    }
    
    private readonly ApplicationContext _parentContext;

    private List<Type> _extenders = new List<Type> ();
    private List<Type> _users = new List<Type> ();
    private List<Type> _potentialTargets = new List<Type> ();
    private List<Type> _completeInterfaces = new List<Type> ();

    public ApplicationContextBuilder (ApplicationContext parentContext)
    {
      _parentContext = parentContext;
    }

    public ApplicationContextBuilder AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      foreach (Type t in assembly.GetTypes())
      {
        if (!t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
          AddType (t);
      }
      return this;
    }

    public ApplicationContextBuilder AddType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (type.IsDefined (typeof (ExtendsAttribute), false))
        _extenders.Add (type);
      if (type.IsDefined (typeof (UsesAttribute), true))
        _users.Add (type);
      if (type.IsDefined (typeof (CompleteInterfaceAttribute), false))
        _completeInterfaces.Add (type);

      _potentialTargets.Add (type);

      return this;
    }

    public ApplicationContext Analyze ()
    {
      ApplicationContext result = new ApplicationContext (_parentContext);
      foreach (Type extender in _extenders)
        AnalyzeMixin (extender, result);
      foreach (Type user in _users)
        AnalyzeMixinApplications (user, result);
      foreach (Type completeInterface in _completeInterfaces)
        AnalyzeCompleteInterface (completeInterface, result);
      return result;
    }

    private void AnalyzeMixin (Type mixinType, ApplicationContext targetContext)
    {
      foreach (ExtendsAttribute mixinAttribute in mixinType.GetCustomAttributes (typeof (ExtendsAttribute), false))
        targetContext.GetOrAddClassContext (mixinAttribute.TargetType).AddMixin (mixinType);
    }

    private void AnalyzeMixinApplications (Type targetType, ApplicationContext targetContext)
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

    private void EnsureNoUsesDuplicates (UsesAttribute[] usesAttributes, Type targetType)
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

    private void ApplyMixinToClassContext (ClassContext classContext, Type mixinType, IEnumerable<Type> explicitDependencies)
    {
      if (!classContext.ContainsMixin (mixinType))
      {
        MixinContext mixinContext = classContext.GetOrAddMixinContext (mixinType);
        foreach (Type additionalDependency in explicitDependencies)
          mixinContext.AddExplicitDependency (additionalDependency);
      }
    }

    private void AnalyzeCompleteInterface (Type completeInterfaceType, ApplicationContext targetContext)
    {
      foreach (CompleteInterfaceAttribute ifaceAttribute in completeInterfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
        targetContext.GetOrAddClassContext (ifaceAttribute.TargetType).AddCompleteInterface (completeInterfaceType);
    }
  }
}
