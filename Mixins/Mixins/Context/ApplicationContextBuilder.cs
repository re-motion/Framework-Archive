using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mixins;
using Rubicon.Collections;
using Mixins.Definitions.Building;
using Rubicon.Reflection;
using Rubicon.Utilities;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;

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

    private class InternalBuilder
    {
      private readonly ApplicationContext _parentContext;
      private readonly Set<Type> _extenders;
      private readonly Set<Type> _users;
      private readonly Set<Type> _potentialTargets;
      private readonly Set<Type> _completeInterfaces;

      private ApplicationContext _builtContext;

      public InternalBuilder (ApplicationContext parentContext, Set<Type> extenders, Set<Type> users, Set<Type> potentialTargets,
          Set<Type> completeInterfaces)
      {
        _parentContext = parentContext;
        _extenders = extenders;
        _completeInterfaces = completeInterfaces;
        _potentialTargets = potentialTargets;
        _users = users;

        Analyze ();
      }

      public ApplicationContext BuiltContext
      {
        get { return _builtContext; }
      }

      public void Analyze ()
      {
        _builtContext = new ApplicationContext (_parentContext);
        foreach (Type extender in _extenders)
          AnalyzeExtender (extender);
        foreach (Type user in _users)
          AnalyzeUser (user);
        foreach (Type completeInterface in _completeInterfaces)
          AnalyzeCompleteInterface (completeInterface);
        foreach (Type type in _potentialTargets)
          AnalyzeInheritedMixins (type);
      }

      private void AnalyzeExtender (Type extender)
      {
        foreach (ExtendsAttribute mixinAttribute in extender.GetCustomAttributes (typeof (ExtendsAttribute), false))
          ApplyMixinToClassContext (_builtContext.GetOrAddClassContext (mixinAttribute.TargetType), extender, Type.EmptyTypes);
      }

      private void AnalyzeUser (Type user)
      {
        ClassContext classContext = _builtContext.GetOrAddClassContext (user);
        foreach (UsesAttribute usesAttribute in user.GetCustomAttributes (typeof (UsesAttribute), false))
          ApplyMixinToClassContext (classContext, usesAttribute.MixinType, usesAttribute.AdditionalDependencies);
      }

      private void ApplyMixinToClassContext (ClassContext classContext, Type mixinType, IEnumerable<Type> explicitDependencies)
      {
        if (AlreadyAppliedSame (mixinType, classContext))
        {
          Type typeForMessage = mixinType;
          if (typeForMessage.IsGenericType)
            typeForMessage = typeForMessage.GetGenericTypeDefinition();
          string message = string.Format ("Two instances of mixin {0} are configured for target type {1}.",
              typeForMessage.FullName, classContext.Type.FullName);
          throw new ConfigurationException (message);
        }
        else {
          MixinContext mixinContext = classContext.AddMixin (mixinType);
          foreach (Type additionalDependency in explicitDependencies)
            mixinContext.AddExplicitDependency (additionalDependency);
        }
      }

      private void AnalyzeCompleteInterface (Type completeInterfaceType)
      {
        foreach (CompleteInterfaceAttribute ifaceAttribute in completeInterfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
          _builtContext.GetOrAddClassContext (ifaceAttribute.TargetType).AddCompleteInterface (completeInterfaceType);
      }

      private void AnalyzeInheritedMixins (Type targetType)
      {
        Type currentBaseType = targetType.BaseType;
        while (currentBaseType != null)
        {
          AnalyzeInheritedMixins (targetType, currentBaseType);
          currentBaseType = currentBaseType.BaseType;
        }
      }

      private void AnalyzeInheritedMixins (Type targetType, Type baseType)
      {
        ClassContext baseTypeContext = _builtContext.GetClassContext (baseType);
        if (baseTypeContext != null)
        {
          foreach (MixinContext baseMixinContext in baseTypeContext.Mixins)
          {
            ClassContext targetContext = _builtContext.GetOrAddClassContext (targetType);
            if (!AlreadyAppliedSameOrDerived (baseMixinContext.MixinType, targetContext))
              baseMixinContext.CloneAndAddTo (targetContext);
          }
        }
      }

      private bool AlreadyAppliedSame (Type mixinType, ClassContext contextToCheck)
      {
        foreach (MixinContext mixin in contextToCheck.Mixins)
        {
          if (ReflectionUtility.IsSameTypeIgnoreGenerics (mixinType, mixin.MixinType))
            return true;
        }
        return false;
      }

      private bool AlreadyAppliedSameOrDerived (Type mixinType, ClassContext contextToCheck)
      {
        foreach (MixinContext mixin in contextToCheck.Mixins)
        {
          if (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (mixin.MixinType, mixinType))
            return true;
        }
        return false;
      }
    }

    private readonly ApplicationContext _parentContext;

    private Set<Type> _extenders = new Set<Type>();
    private Set<Type> _users = new Set<Type>();
    private Set<Type> _potentialTargets = new Set<Type>();
    private Set<Type> _completeInterfaces = new Set<Type>();
    private Set<Type> _allTypes = new Set<Type> ();

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

      // When analyzing types, we want type definitions, not specializations
      if (type.IsGenericType)
        type = type.GetGenericTypeDefinition ();

      if (!_allTypes.Contains (type))
      {
        _allTypes.Add (type);

        if (type.IsDefined (typeof (ExtendsAttribute), false))
          _extenders.Add (type);
        if (type.IsDefined (typeof (UsesAttribute), false))
          _users.Add (type);
        if (type.IsDefined (typeof (CompleteInterfaceAttribute), false))
          _completeInterfaces.Add (type);

        _potentialTargets.Add (type);

        if (type.BaseType != null)
          AddType (type.BaseType);
      }

      return this;
    }

    public ApplicationContext Analyze ()
    {
      return new InternalBuilder (_parentContext, _extenders, _users, _potentialTargets, _completeInterfaces).BuiltContext;
    }
  }
}
