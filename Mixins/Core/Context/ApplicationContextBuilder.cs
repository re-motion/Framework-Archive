using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins;
using Rubicon.Collections;
using Rubicon.Reflection;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.Context
{
	/// <summary>
	/// Provides support for building mixin configuration data for an application from the declarative configuration attributes
	/// (<see cref="ContainsMixinInfoAttribute"/>, <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, <see cref="CompleteInterfaceAttribute"/>,
	/// and <see cref="IgnoreForMixinConfigurationAttribute"/>).
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
  public class ApplicationContextBuilder
  {
		/// <summary>
		/// Builds a new <see cref="ApplicationContext"/> from the declarative configuration information in the given assemblies.
		/// </summary>
		/// <param name="parentContext">The parent context to derive the new context from (can be <see langword="null"/>).</param>
		/// <param name="assemblies">The assemblies to be scanned for declarative mixin information (if they are marked with the
		/// <see cref="ContainsMixinInfoAttribute"/>).</param>
		/// <returns>An application context inheriting from <paramref name="parentContext"/> and incorporating the configuration information
		/// held by the given assemblies.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static ApplicationContext BuildContextFromAssemblies (ApplicationContext parentContext, IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      // ApplicationContextBuilder will not overwrite any existing class contexts, but instead augment them with any new definitions.
      // This is exactly what we want for the assemblies, since all of these are of equal priority. However, we want to replace any conflicting
			// contexts inherited from the parent context.

      // Therefore, first analyze the assemblies into a temporary context without replacements:
      ApplicationContextBuilder tempContextBuilder = new ApplicationContextBuilder (null);
      foreach (Assembly assembly in assemblies)
        tempContextBuilder.AddAssembly (assembly);

      ApplicationContext tempContext = tempContextBuilder.BuildContext ();

      // Then, add them to the resulting context, replacing the respective inherited class contexts:
      ApplicationContext fullContext = new ApplicationContext (parentContext);
      tempContext.CopyTo (fullContext);
      return fullContext;
    }

		/// <summary>
		/// Builds a new <see cref="ApplicationContext"/> from the declarative configuration information in the given assemblies.
		/// </summary>
		/// <param name="parentContext">The parent context to derive the new context from (can be <see langword="null"/>).</param>
		/// <param name="assemblies">The assemblies to be scanned for declarative mixin information (if they are marked with the
		/// <see cref="ContainsMixinInfoAttribute"/>).</param>
		/// <returns>An application context inheriting from <paramref name="parentContext"/> and incorporating the configuration information
		/// held by the given assemblies.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static ApplicationContext BuildContextFromAssemblies (ApplicationContext parentContext, params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildContextFromAssemblies (parentContext, (IEnumerable<Assembly>) assemblies);
    }

		/// <summary>
		/// Builds a new <see cref="ApplicationContext"/> from the declarative configuration information in the given assemblies without inheriting
		/// from a parent context.
		/// </summary>
		/// <param name="assemblies">The assemblies to be scanned for declarative mixin information (if they are marked with the
		/// <see cref="ContainsMixinInfoAttribute"/>).</param>
		/// <returns>An application context incorporating the configuration information held by the given assemblies.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static ApplicationContext BuildContextFromAssemblies (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildContextFromAssemblies (null, (IEnumerable<Assembly>) assemblies);
    }

		/// <summary>
		/// Builds a new <see cref="ApplicationContext"/> containing the given class contexts (replacing conflicting inherited ones, if any).
		/// </summary>
		/// <param name="parentContext">The parent context to derive the new context from (can be <see langword="null"/>).</param>
		/// <param name="classContexts">The class contexts to be contained in the new application context.</param>
		/// <returns>An application context inheriting from <paramref name="parentContext"/> and incorporating the given class contexts.</returns>
		public static ApplicationContext BuildContextFromClasses (ApplicationContext parentContext, params ClassContext[] classContexts)
		{
			ArgumentUtility.CheckNotNull ("classContexts", classContexts);

			ApplicationContext context = new ApplicationContext (parentContext);

			foreach (ClassContext classContext in classContexts)
				context.AddOrReplaceClassContext (classContext);

			return context;
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
        {
          ClassContext classContext = _builtContext.GetOrAddClassContext (ifaceAttribute.TargetType);
          classContext.AddCompleteInterface (completeInterfaceType);
          _builtContext.RegisterInterface (completeInterfaceType, classContext);
        }
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

		/// <summary>
		/// Initializes a new <see cref="ApplicationContextBuilder"/>, which can be used to collect assemblies and types with declarative
		/// mixin configuration attributes in order to build an <see cref="ApplicationContext"/>.
		/// </summary>
		/// <param name="parentContext">The parent context used when this instance builds a new <see cref="ApplicationContext"/>.</param>
    public ApplicationContextBuilder (ApplicationContext parentContext)
    {
      _parentContext = parentContext;
    }

		/// <summary>
		/// Scans the given assembly for declarative mixin configuration information and stores the information for a later call to <see cref="BuildContext"/>.
		/// The mixin configuration information of types marked with the <see cref="IgnoreForMixinConfigurationAttribute"/> will be ignored.
		/// </summary>
		/// <param name="assembly">The assembly to be scanned.</param>
		/// <returns>A reference to this <see cref="ApplicationContextBuilder"/> object.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
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

		/// <summary>
		/// Scans the given type for declarative mixin configuration information and stores the information for a later call to <see cref="BuildContext"/>.
		/// The type will be scanned whether or not is is marked with the <see cref="IgnoreForMixinConfigurationAttribute"/>.
		/// </summary>
		/// <param name="type">The type to be scanned. This must be a non-generic type or a generic type definition. Closed generic types are not
		/// supported to be scanned.</param>
		/// <returns>A reference to this <see cref="ApplicationContextBuilder"/> object.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The given type is a closed generic type and not a generic type definition.</exception>
    public ApplicationContextBuilder AddType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

			if (type.IsGenericType && !type.IsGenericTypeDefinition)
				throw new ArgumentException ("Type must be non-generic or a generic type definition.", "type");

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
				{
					// When analyzing types, we want type definitions, not specializations
					if (type.BaseType.IsGenericType)
						AddType (type.BaseType.GetGenericTypeDefinition());
					else
						AddType (type.BaseType);

				}
      }

      return this;
    }

		/// <summary>
		/// Analyzes the information added so far to this builder and creates a new <see cref="ApplicationContext"/> from that data.
		/// </summary>
		/// <returns>An <see cref="ApplicationContext"/> derived from the context specified in the builder's constructor containing
		/// <see cref="ClassContext"/> and <see cref="MixinContext"/> objects based on the information added so far.</returns>
    public ApplicationContext BuildContext ()
    {
      return new InternalBuilder (_parentContext, _extenders, _users, _potentialTargets, _completeInterfaces).BuiltContext;
    }
  }
}
