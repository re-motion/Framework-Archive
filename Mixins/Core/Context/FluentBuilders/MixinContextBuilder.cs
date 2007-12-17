using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Assists <see cref="MixinConfigurationBuilder"/> by providing a fluent interface for building <see cref="MixinContext"/> objects.
  /// </summary>
  public class MixinContextBuilder
  {
    private readonly ClassContextBuilder _parent;
    private readonly Type _mixinType;
    private readonly List<Type> _dependencies = new List<Type>();

    public MixinContextBuilder (ClassContextBuilder parent, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _parent = parent;
      _mixinType = mixinType;
    }

    /// <summary>
    /// Gets the <see cref="ClassContextBuilder"/> used for creating this <see cref="MixinContextBuilder"/>.
    /// </summary>
    /// <value>This object's <see cref="ClassContextBuilder"/>.</value>
    public ClassContextBuilder Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Gets mixin type configured by this object.
    /// </summary>
    /// <value>The mixin type configured by this object.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// Gets the base call dependencies collected so far.
    /// </summary>
    /// <value>The base call dependencies collected so far by this object.</value>
    public IEnumerable<Type> Dependencies
    {
      get { return _dependencies; }
    }

    /// <summary>
    /// Collects a dependency for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <param name="requiredMixin">The mixin required by the configured <see cref="MixinType"/>.</param>
    /// <returns>This object for further configuration of <see cref="MixinType"/>.</returns>
    public virtual MixinContextBuilder WithDependency (Type requiredMixin)
    {
      ArgumentUtility.CheckNotNull ("requiredMixin", requiredMixin);
      _dependencies.Add (requiredMixin);
      return this;
    }

    /// <summary>
    /// Collects a dependency for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TRequiredMixin">The mixin (or an interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of <see cref="MixinType"/>.</returns>
    public virtual MixinContextBuilder WithDependency<TRequiredMixin> ()
    {
      return WithDependency (typeof (TRequiredMixin));
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <param name="requiredMixins">The mixins (or interfaces) required by the configured <see cref="MixinType"/>.</param>
    /// <returns>This object for further configuration of <see cref="MixinType"/>.</returns>
    public virtual MixinContextBuilder WithDependencies (params Type[] requiredMixins)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("requiredMixins", requiredMixins);
      foreach (Type requiredMixin in requiredMixins)
        WithDependency (requiredMixin);
      return this;
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin2">The second mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of <see cref="MixinType"/>.</returns>
    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin2">The second mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin3">The third mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of <see cref="MixinType"/>.</returns>
    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2, TMixin3> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Builds a mixin context with the data collected so far for the <see cref="MixinType"/>.
    /// </summary>
    /// <param name="classContext">The class context to build the mixin context with.</param>
    /// <returns>A <see cref="MixinContext"/> for the <see cref="MixinType"/> holding all mixin configuration data collected so far.</returns>
    public virtual MixinContext BuildMixinContext (ClassContext classContext)
    {
      MixinContext mixinContext = classContext.AddMixin (_mixinType);
      foreach (Type dependency in _dependencies)
        mixinContext.AddExplicitDependency (dependency);
      return mixinContext;
    }

    #region Parent members

    /// <summary>
    /// Clears all mixin configuration for the <see cref="Parent"/>'s <see cref="ClassContextBuilder.TargetType"/>. This causes the target type to ignore
    /// all mixin configuration data from its
    /// <see cref="ClassContextBuilder.ParentContext"/> and also resets all information collected so far for the class by this object.
    /// </summary>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder Clear ()
    {
      return _parent.Clear();
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      return _parent.AddMixin (mixinType);
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return _parent.AddMixin<TMixin>();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      return _parent.AddMixins (mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2> ();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2, TMixin3>();
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="ClassContextBuilder.TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the <see cref="ClassContextBuilder.ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixin (Type mixinType)
    {
      return _parent.EnsureMixin (mixinType);
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="ClassContextBuilder.TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the <see cref="ClassContextBuilder.ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixin<TMixin> ()
    {
      return _parent.EnsureMixin<TMixin>();
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ClassContextBuilder.ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins (params Type[] mixinTypes)
    {
      return _parent.EnsureMixins (mixinTypes);
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ClassContextBuilder.ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2> ()
    {
      return _parent.EnsureMixins<TMixin1, TMixin2>();
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ClassContextBuilder.ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.EnsureMixins<TMixin1, TMixin2, TMixin3>();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      return _parent.AddOrderedMixins (mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2> ();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2, TMixin3> ();
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceType">The type to collect as a complete interface.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      return _parent.AddCompleteInterface (interfaceType);
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface">The type to collect as a complete interface.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return _parent.AddCompleteInterface<TInterface> ();
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceTypes">The types to collect as complete interfaces.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      return _parent.AddCompleteInterfaces (interfaceTypes);
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2> ();
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface3">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ();
    }

    /// <summary>
    /// Collects the given class context to inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ClassContextBuilder.ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <param name="contextOfTargetTypeToInheritFrom">The class context to inherit from.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom (ClassContext contextOfTargetTypeToInheritFrom)
    {
      return _parent.InheritFrom (contextOfTargetTypeToInheritFrom);
    }

    /// <summary>
    /// Collects the class context of the given type from the <see cref="Parent"/>'s <see cref="MixinConfigurationBuilder.ParentConfiguration"/> to
    /// inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ClassContextBuilder.ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <param name="targetTypeToInheritFrom">The target type to inherit from.</param>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom (Type targetTypeToInheritFrom)
    {
      return _parent.InheritFrom (targetTypeToInheritFrom);
    }

    /// <summary>
    /// Collects the class context of the given type from the <see cref="Parent"/>'s <see cref="MixinConfigurationBuilder.ParentConfiguration"/> to
    /// inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ClassContextBuilder.ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <typeparam name="TTypeToInheritFrom">The target type to inherit from.</typeparam>
    /// <returns>This object for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom<TTypeToInheritFrom> ()
    {
      return _parent.InheritFrom<TTypeToInheritFrom>();
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <param name="mixinConfiguration">The mixin configuration to build the class context with.</param>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="ClassContextBuilder.TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext (MixinConfiguration mixinConfiguration)
    {
      return _parent.BuildClassContext (mixinConfiguration);
    }

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      return _parent.ForClass (targetType);
    }

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return _parent.ForClass<TTargetType> ();
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration ();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope ();
    }
    #endregion
  }
}