using System;
using System.Runtime.Serialization;
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
    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>, or <paramref name="targetType"/> itself if no
    /// mixin configuration exists for the type in <see cref="MixinConfiguration.ActiveContext"/>.
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
    /// Note that the factory will not create derived types for types not currently having a mixin configuration. This means that types returned
    /// by the factory can <b>not</b> always be treated as derived types. See <see cref="GetConcreteType(Type,GenerationPolicy)"/> on how to
    /// force generation of a derived type.
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
      return GetConcreteType (targetType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">Base type for which a mixed type should be retrieved.</param>
    /// <param name="generationPolicy">Defines whether to force generation of a type even if no <see cref="MixinConfiguration"/> is currently available
    /// for the given type.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetType"/>, or the type itself; depending on the
    /// <paramref name="generationPolicy"/> and the <see cref="MixinConfiguration.ActiveContext"/>.</returns>
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
    /// Note that the factory can create empty mixin configurations for types not currently having a mixin configuration, depending on the
    /// <paramref name="generationPolicy"/>. With <see cref="GenerationPolicy.ForceGeneration"/>, types returned by the factory can always be treated
    /// as derived types.
    /// </para>
    /// <para>
    /// The returned type provides the same constructors as <paramref name="targetType"/> does and can thus be instantiated, e.g. via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. When this happens, all the mixins associated with the generated type are also
    /// instantiated and configured to be used with the target instance. If you need to supply pre-created mixin instances for an object, use
    /// <see cref="MixedTypeInstantiationScope"/>. See <see cref="ObjectFactory"/> for a simpler way to immediately create instances of mixed types.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      TargetClassDefinition configuration = GetActiveConfiguration (targetType, generationPolicy);
      if (configuration == null)
        return targetType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
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
    /// Use <see cref="GetActiveConfiguration(Type,GenerationPolicy)"/> to force generation of an empty configuration if none currently
    /// exists for the given type in <see cref="MixinConfiguration.ActiveContext"/>.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return GetActiveConfiguration (targetType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type.</returns>
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
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in <see cref="MixinConfiguration.ActiveContext"/>.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return GetConfiguration (targetType, MixinConfiguration.ActiveContext, generationPolicy);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="applicationContext">The <see cref="ApplicationContext"/> to use.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// the given type.</returns>
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
    /// Use <see cref="GetConfiguration(Type,ApplicationContext,GenerationPolicy)"/> to force generation of an empty configuration if none currently
    /// exists for the given type.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetType, ApplicationContext applicationContext)
    {
      return GetConfiguration (targetType, applicationContext, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="applicationContext">The <see cref="ApplicationContext"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/>.</returns>
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
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in <paramref name="applicationContext"/>.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetType, ApplicationContext applicationContext, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      ClassContext context = applicationContext.GetClassContext (targetType);
      if (context == null && generationPolicy == GenerationPolicy.ForceGeneration)
        context = new ClassContext (targetType);

      if (context == null)
        return null;
      else
      {
        if (targetType.IsGenericType)
        {
          Assertion.IsTrue (context.Type.IsGenericTypeDefinition);
          context = context.SpecializeWithTypeArguments (targetType.GetGenericArguments());
        }

        return TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
      }
    }

    /// <summary>
    /// Initializes a mixin target instance which was created without its constructor having been called.
    /// </summary>
    /// <param name="mixinTarget">The mixin target to initialize.</param>
    /// <exception cref="ArgumentNullException">The mixin target is <see langword="null"/>.</exception>
    /// <remarks>This method is useful when a mixin target instance is created via <see cref="FormatterServices.GetSafeUninitializedObject"/>.</remarks>
    public static void InitializeUnconstructedInstance (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ConcreteTypeBuilder.Current.InitializeUnconstructedInstance (mixinTarget);
    }
  }
}