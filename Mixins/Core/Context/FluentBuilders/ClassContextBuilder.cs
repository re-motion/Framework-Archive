using System;
using System.Collections.Generic;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Assists <see cref="MixinConfigurationBuilder"/> by providing a fluent interface for building <see cref="ClassContext"/> objects.
  /// </summary>
  public class ClassContextBuilder
  {
    private readonly MixinConfigurationBuilder _parent;
    private readonly Type _targetType;
    private readonly Dictionary<Type, MixinContextBuilder> _mixinContextBuilders = new Dictionary<Type, MixinContextBuilder> ();
    private readonly List<Type> _completeInterfaces = new List<Type> ();
    private readonly List<ClassContext> _typesToInheritFrom = new List<ClassContext> ();

    private ClassContext _parentContext;

    public ClassContextBuilder (MixinConfigurationBuilder parent, Type targetType, ClassContext parentContext)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      _parent = parent;
      _targetType = targetType;
      _parentContext = parentContext;
    }

    /// <summary>
    /// Gets the <see cref="MixinConfigurationBuilder"/> used for creating this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>This object's <see cref="MixinConfigurationBuilder"/>.</value>
    public MixinConfigurationBuilder Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Gets the type configured by this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>The target type configured by this object.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets the parent context for the target type configured by this object. The parent context usually holds the target type's mixin configuration
    /// in the <see cref="Parent"/>'s <see cref="MixinConfigurationBuilder.ParentConfiguration"/>; the target type will automatically contain all
    /// mixins  and complete interfaces from that context unless the <see cref="Clear"/> method is called.
    /// </summary>
    /// <value>The parent context whose mixin configuration will be taken over by the <see cref="TargetType"/>.</value>
    public ClassContext ParentContext
    {
      get { return _parentContext; }
    }

    /// <summary>
    /// Gets the mixin context builders collected so far.
    /// </summary>
    /// <value>The mixin context builders collected so far by this object.</value>
    public IEnumerable<MixinContextBuilder> MixinContextBuilders
    {
      get { return _mixinContextBuilders.Values; }
    }

    /// <summary>
    /// Gets the complete interfaces collected so far.
    /// </summary>
    /// <value>The complete interfaces collected so far by this object.</value>
    public IEnumerable<Type> CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    /// <summary>
    /// Gets the inherited types collected so far.
    /// </summary>
    /// <value>The inherited types collected so far by this object.</value>
    public IEnumerable<ClassContext> TypesToInheritFrom
    {
      get { return _typesToInheritFrom; }
    }

    /// <summary>
    /// Clears all mixin configuration for the <see cref="TargetType"/>. This causes the target type to ignore all mixin configuration data from its
    /// <see cref="ParentContext"/> and also resets all information collected so far for the class by this object.
    /// </summary>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder Clear ()
    {
      _parentContext = null;
      _mixinContextBuilders.Clear();
      _completeInterfaces.Clear();
      _typesToInheritFrom.Clear();
      return this;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      MixinContextBuilder mixinContextBuilder = new MixinContextBuilder (this, mixinType);
      _mixinContextBuilders.Add (mixinType, mixinContextBuilder);
      return mixinContextBuilder;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return AddMixin (typeof (TMixin));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        AddMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the <see cref="ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      if (!ContainsMixin (mixinType))
        AddMixin (mixinType);
      return this;
    }

    private bool ContainsMixin (Type mixinType)
    {
      if (_mixinContextBuilders.ContainsKey (mixinType))
        return true;

      foreach (ClassContext inheritedContext in TypesToInheritFrom)
      {
        if (inheritedContext.ContainsMixin (mixinType))
          return true;
      }

      if (ParentContext != null && ParentContext.ContainsMixin (mixinType))
        return true;

      return false;
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the <see cref="ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixin<TMixin>()
    {
      return EnsureMixin (typeof (TMixin));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        EnsureMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2> ()
    {
      return EnsureMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the <see cref="ParentContext"/> (unless <see cref="Clear"/> was called) or inherited from a type added via
    /// <see cref="InheritFrom(ClassContext)"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return EnsureMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      Type lastMixinType = null;
      foreach (Type mixinType in mixinTypes)
      {
        MixinContextBuilder mixinContextBuilder = AddMixin (mixinType);
        if (lastMixinType != null)
          mixinContextBuilder.WithDependency (lastMixinType);
        lastMixinType = mixinType;
      }
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceType">The type to collect as a complete interface.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      _completeInterfaces.Add (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface">The type to collect as a complete interface.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return AddCompleteInterface (typeof (TInterface));
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceTypes">The types to collect as complete interfaces.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("interfaceTypes", interfaceTypes);
      foreach (Type interfaceType in interfaceTypes)
        AddCompleteInterface (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2));
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface3">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2), typeof (TInterface3));
    }

    /// <summary>
    /// Collects the given class context to inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <param name="contextOfTargetTypeToInheritFrom">The class context to inherit from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom (ClassContext contextOfTargetTypeToInheritFrom)
    {
      ArgumentUtility.CheckNotNull ("contextOfTargetTypeToInheritFrom", contextOfTargetTypeToInheritFrom);
      _typesToInheritFrom.Add (contextOfTargetTypeToInheritFrom);
      return this;
    }

    /// <summary>
    /// Collects the class context of the given type from the <see cref="Parent"/>'s <see cref="MixinConfigurationBuilder.ParentConfiguration"/> to
    /// inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <param name="targetTypeToInheritFrom">The target type to inherit from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom (Type targetTypeToInheritFrom)
    {
      ArgumentUtility.CheckNotNull ("targetTypeToInheritFrom", targetTypeToInheritFrom);
      ClassContext contextForType = Parent.ParentConfiguration.GetClassContext (targetTypeToInheritFrom);
      if (contextForType != null)
        return InheritFrom (contextForType);
      else
        return this;
    }

    /// <summary>
    /// Collects the class context of the given type from the <see cref="Parent"/>'s <see cref="MixinConfigurationBuilder.ParentConfiguration"/> to
    /// inherit mixin configuration data from it. When mixin configuration data is inherited, it can be overridden
    /// by mixin configuration data from the <see cref="ParentContext"/> or by data explicitly added via <see cref="AddMixin"/>. A mixin overrides
    /// another mixin if it is of the same or a derived type (or a generic specialization of the type).
    /// </summary>
    /// <typeparam name="TTypeToInheritFrom">The target type to inherit from.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder InheritFrom<TTypeToInheritFrom> ()
    {
      return InheritFrom (typeof (TTypeToInheritFrom));
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinConfiguration">The mixin configuration to build the class context with.</param>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext (MixinConfiguration mixinConfiguration)
    {
      ClassContext classContext;
      if (ParentContext != null)
        classContext = ParentContext.CloneForSpecificType (_targetType);
      else
        classContext = new ClassContext (_targetType);

      foreach (MixinContextBuilder mixinContextBuilder in MixinContextBuilders)
        mixinContextBuilder.BuildMixinContext (classContext);
      foreach (Type completeInterface in CompleteInterfaces)
        classContext.AddCompleteInterface (completeInterface);
      foreach (ClassContext typeToInheritFrom in TypesToInheritFrom)
        classContext.InheritFrom (typeToInheritFrom);
      mixinConfiguration.AddOrReplaceClassContext (classContext);
      return classContext;
    }

    #region Parent members

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
      return _parent.ForClass<TTargetType>();
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope();
    }
    #endregion
  }
}