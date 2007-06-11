using System;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.Validation;
using Rubicon.Utilities;
using Mixins.Context;

namespace Mixins
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
  public static class TypeFactory
  {
    /// <summary>
    /// Initializes an instance of a newly created mixed type.
    /// </summary>
    /// <param name="instance">The instance to be initialized.</param>
    /// <exception cref="ArgumentTypeException">The instance is not an instance of a mixed type.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// This is mainly for internal purposes, users should use the <see cref="ObjectFactory"/>
    /// class to instantiate mixed types. However, if you employ the <see cref="TypeFactory"/> to retrieve a mixed type and later instantiate it,
    /// instances of this type must be initialized by the <see cref="TypeFactory"/> class before being used.
    /// </remarks>
    public static void InitializeMixedInstance (object instance)
    {
      IMixinTarget mixinTarget = ArgumentUtility.CheckNotNullAndType<IMixinTarget> ("instance", instance);
      ConcreteTypeBuilder.Current.Scope.InitializeInstance (mixinTarget);
    }

    /// <summary>
    /// Initializes an instance of a newly created mixed type and provides instantiations of some of the mixins the type is configured with.
    /// </summary>
    /// <param name="instance">The instance to be initialized.</param>
    /// <param name="mixinInstances">The mixin instances to be used when initializing the target instance. Each element of this array must be
    /// of a mixin type as configured in the target's original <see cref="ClassContext"/>.</param>
    /// <exception cref="ArgumentTypeException">The instance is not an instance of a mixed type.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="mixinInstances"/> parameter contains at least one mixin instance which is not of
    /// a mixin type as configured in the target's original <see cref="ClassContext"/>.</exception>
    /// <remarks>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="InitializeMixedInstance"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// This is mainly for internal purposes, users should use the <see cref="ObjectFactory"/>
    /// class to instantiate mixed types. However, if you employ the <see cref="TypeFactory"/> to retrieve a mixed type and later instantiate it,
    /// instances of this type must be initialized by the <see cref="TypeFactory"/> class before being used.
    /// </para>
    /// </remarks>
    public static void InitializeMixedInstanceWithMixins (object instance, object[] mixinInstances)
    {
      IMixinTarget mixinTarget = ArgumentUtility.CheckNotNullAndType<IMixinTarget> ("instance", instance);
      ConcreteTypeBuilder.Current.Scope.InitializeInstanceWithMixins (mixinTarget, mixinInstances);
    }

    // Instances of the type returned by this method must be initialized with <see cref="InitializeMixedInstance"/>
    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">Base type for which a mixed type should be retrieved.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetType"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="BaseClassDefinition"/> object impossible.</exception>
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
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. However, before using instances of the type, they must be initialized via
    /// <see cref="InitializeMixedInstance"/>  or <see cref="InitializeMixedInstanceWithMixins"/>. See <see cref="ObjectFactory"/> for a simpler
    /// way to immediately create instances of mixed types.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      BaseClassDefinition configuration = GetActiveConfiguration (targetType);
      if (configuration == null)
        return targetType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    /// <summary>
    /// Returns a non-null <see cref="BaseClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <returns>A non-null <see cref="BaseClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="BaseClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="BaseClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Note that this method creates an empty but valid <see cref="BaseClassDefinition"/> for types that do not have a mixin configuration in
    /// <see cref="MixinConfiguration.ActiveContext"/>.
    /// </para>
    /// </remarks>
    public static BaseClassDefinition GetActiveConfiguration (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ClassContext context = MixinConfiguration.ActiveContext.GetClassContext (targetType);
      if (context == null)
        context = new ClassContext (targetType);
      return BaseClassDefinitionCache.Current.GetBaseClassDefinition (context);
    }
  }
}