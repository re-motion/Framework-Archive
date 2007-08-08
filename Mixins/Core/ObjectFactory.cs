using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Validation;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{

  /// <summary>
  /// Provides support for instantiating type which are combined with mixins.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code, and this type is then instantiated.
  /// The <see cref="ObjectFactory"/> class provides a simple API to creating and instantiating these mixed types. If a mixed type should be
  /// created without being instantiated, refer to the <see cref="TypeFactory"/> class.
  /// </para>
  /// <para>
  /// The <see cref="ObjectFactory"/> class uses the mixin configuration defined by <see cref="MixinConfiguration.ActiveContext"/>. Use the 
  /// <see cref="MixinConfiguration"/> class if the configuration needs to be adapted.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public static class ObjectFactory
  {
    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.</exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// The <see cref="Create{T}()"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> ()
    {
      return Create<T> (GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/>, allowing to specify a type generation policy.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.</exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// The <see cref="Create{T}(GenerationPolicy)"/> method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> (GenerationPolicy generationPolicy)
    {
      return CreateWithMixinInstances<T> (generationPolicy);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="baseType"/>.
    /// </summary>
    /// <param name="baseType">The target type a mixed instance of which should be created.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="baseType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <paramref name="baseType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <paramref name="baseType"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="baseType"/> is an interface and it cannot be determined which class
    /// to instantiate.</exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// The <see cref="Create(Type)"/> method supports the creation of instances from their complete interfaces: <paramref name="baseType"/> can be
    /// an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      return Create (baseType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="baseType"/>.
    /// </summary>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin
    /// configuration.</param>
    /// <param name="baseType">The target type a mixed instance of which should be created.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="baseType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <paramref name="baseType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <paramref name="baseType"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="baseType"/> is an interface and it cannot be determined which class
    /// to instantiate.</exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// The <see cref="Create(Type, GenerationPolicy)"/> method supports the creation of instances from their complete interfaces:
    /// <paramref name="baseType"/> can be an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (Type baseType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      return CreateWithMixinInstances (baseType, generationPolicy);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/> and integrates a number of prepared mixin instances.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="mixinInstances">The pre-instantiated mixin instances to integrate into the mixed instance.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="mixinInstances"/> parameter contains at least one mixin instance which is not of
    /// a mixin type as configured in the target's original <see cref="ClassContext"/>.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="Create{T}()"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// The <see cref="CreateWithMixinInstances{T}(object[])"/> method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> CreateWithMixinInstances<T> (params object[] mixinInstances)
    {
      return CreateWithMixinInstances<T> (GenerationPolicy.GenerateOnlyIfConfigured, mixinInstances);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/> and integrates a number of prepared mixin instances.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="mixinInstances">The pre-instantiated mixin instances to integrate into the mixed instance.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="mixinInstances"/> parameter contains at least one mixin instance which is not of
    /// a mixin type as configured in the target's original <see cref="ClassContext"/>.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="Create{T}(GenerationPolicy)"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// The <see cref="CreateWithMixinInstances{T}(GenerationPolicy, object[])"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> CreateWithMixinInstances<T> (GenerationPolicy generationPolicy, params object[] mixinInstances)
    {
      return MixedTypeInvokeWithCreator.CreateInvokeWithWrapper<T> (typeof (T), generationPolicy, mixinInstances);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="baseType"/> and integrates a number of prepared mixin instances.
    /// </summary>
    /// <param name="baseType">The target type a mixed instance of which should be created.</param>
    /// <param name="mixinInstances">The pre-instantiated mixin instances to integrate into the mixed instance.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="baseType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <paramref name="baseType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <paramref name="baseType"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="baseType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="mixinInstances"/> parameter contains at least one mixin instance which is not of
    /// a mixin type as configured in the target's original <see cref="ClassContext"/>.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="Create(Type)"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// The <see cref="CreateWithMixinInstances(Type, object[])"/> method supports the creation of instances from their complete interfaces:
    /// <paramref name="baseType"/> can be an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> CreateWithMixinInstances (Type baseType, params object[] mixinInstances)
    {
      return CreateWithMixinInstances (baseType, GenerationPolicy.GenerateOnlyIfConfigured, mixinInstances);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="baseType"/> and integrates a number of prepared mixin instances.
    /// </summary>
    /// <param name="baseType">The target type a mixed instance of which should be created.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="mixinInstances">The pre-instantiated mixin instances to integrate into the mixed instance.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="baseType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <paramref name="baseType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <paramref name="baseType"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="baseType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="mixinInstances"/> parameter contains at least one mixin instance which is not of
    /// a mixin type as configured in the target's original <see cref="ClassContext"/>.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="Create(Type)"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// The <see cref="CreateWithMixinInstances (Type, GenerationPolicy, object[])"/> method supports the creation of instances from their complete
    /// interfaces: <paramref name="baseType"/> can be an interface registered in the <see cref="MixinConfiguration.ActiveContext"/>. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> CreateWithMixinInstances (Type baseType, GenerationPolicy generationPolicy, params object[] mixinInstances)
    {
      return MixedTypeInvokeWithCreator.CreateInvokeWithWrapper<object> (baseType, generationPolicy, mixinInstances);
    }
  }
}
