using System;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.Validation;
using Rubicon.Utilities;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Provides support for combining mixins and target classes into concrete, "mixed", instantiable types.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code. While the type generation is actually performed
  /// by <see cref="ConcreteTypeBuilder"/>, the <see cref="TypeFactory"/> bridges type generation and mixin configuration and provides the public
  /// API to be used.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> should only be used if <see cref="Type"/> objects are required. If the combined type should be instantiated,
  /// the <see cref="ObjectFactory"/> class should be used instead.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> class uses the mixin configuration defined by <see cref="MixinConfiguration.ActiveContext"/>. Use the 
  /// <see cref="MixinConfiguration"/> class if the configuration needs to be adapted.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public static class TypeFactory
  {
    // Instances of the type returned by this method must be initialized with <see cref="InitializeMixedInstance"/>
    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">Base type for which a mixed type should be retrieved.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetType"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
    /// rule, which makes code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// The type returned by this method is guaranteed to be derived from <paramref name="targetType"/>, but will usually not be the same as
    /// <paramref name="targetType"/>. It manages integration of the mixins currently configured via <see cref="MixinConfiguration.ActiveContext"/>
    /// with the given <paramref name="targetType"/>.
    /// </para>
    /// <para>
    /// Note that the factory will create empty mixin configurations for types not currently having a mixin configuration. This means that types returned
    /// by the factory can always be treated as mixed types, but it might be unnecessarily inefficient to use the <see cref="TypeFactory"/> on such
    /// types.
    /// </para>
    /// <para>
    /// The returned type provides the same constructors as <paramref name="targetType"/> does and can thus be instantiated, e.g. via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. When this happens, all the mixins associated with the generated type are also
    /// instantiated and configured to be used with the target instance. If you need to supply pre-created mixin instances for an object, use
    /// <see cref="MixedTypeInstantiationScope"/>. See <see cref="ObjectFactory"/> for a simpler way to immediately create instances of mixed types.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      TargetClassDefinition configuration = GetActiveConfiguration (targetType);
      if (configuration == null)
        return targetType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    /// <summary>
    /// Returns a non-null <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <returns>A non-null <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Note that this method creates an empty but valid <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in
    /// <see cref="MixinConfiguration.ActiveContext"/>.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      
      return GetConfiguration (targetType, MixinConfiguration.ActiveContext);
    }

    /// <summary>
    /// Returns a non-null <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="applicationContext">The <see cref="ApplicationContext"/> to use.</param>
    /// <returns>A non-null <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Note that this method creates an empty but valid <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in
    /// <paramref name="applicationContext"/>.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetType, ApplicationContext applicationContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      Type typeToLookup;
      if (targetType.IsGenericType)
        typeToLookup = targetType.GetGenericTypeDefinition ();
      else
        typeToLookup = targetType;

      ClassContext context = applicationContext.GetClassContext (typeToLookup);
      if (context == null)
        context = new ClassContext (typeToLookup);

      if (targetType.IsGenericType)
        context = context.SpecializeWithTypeArguments (targetType.GetGenericArguments ());

      return TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
    }
  }
}