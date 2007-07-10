using System;
using Rubicon.Mixins.Utilities.Singleton;
using Rubicon.Mixins.Definitions;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a way to build concrete types for target and mixin classes in mixin configurations and maintains a cache for built types.
  /// </summary>
  public class ConcreteTypeBuilder : ThreadSafeSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;
    // No laziness here - a ModuleBuilder cannot be used by multiple threads at the same time anyway, so using a lazy cache could actually cause
    // errors (depending on how it was implemented)
    private InterlockedCache<ClassDefinition, Type> _typeCache = new InterlockedCache<ClassDefinition, Type>();

    private object _scopeLockObject = new object ();

    /// <summary>
    /// Gets or sets the module scope of this <see cref="ConcreteTypeBuilder"/>. The object returned by this property must not be used by multiple
    /// threads at the same time (or while another thread executes methods on <see cref="ConcreteTypeBuilder"/>). Use the
    /// <see cref="LockAndAccessScope"/> method to access the scope in a thread-safe way.
    /// </summary>
    /// <value>The module scope of this <see cref="ConcreteTypeBuilder"/>.</value>
    public IModuleManager Scope
    {
      get
      {
        lock (_scopeLockObject)
        {
          if (_scope == null)
            _scope = new DynamicProxy.ModuleManager();
          return _scope;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _scope = value;
        }
      }
    }

    /// <summary>
    /// Provides thread-safe access to the module scope of <see cref="ConcreteTypeBuilder"/>, see also <see cref="Scope"/>.
    /// </summary>
    /// <param name="scopeAccessor">A delegate accessing the scope while access to it is locked.</param>
    /// <remarks>This methods locks the scope while executing <paramref name="scopeAccessor"/>. This ensures that no other method of
    /// <see cref="ConcreteTypeBuilder"/> modifies the scope while it is being accessed.</remarks>
    public void LockAndAccessScope (Proc<IModuleManager> scopeAccessor)
    {
      lock (_scopeLockObject)
      {
        scopeAccessor (Scope);
      }
    }

    /// <summary>
    /// Gets the concrete mixed type for the given target class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="configuration">The configuration object for the target class.</param>
    /// <returns>A concrete type with all mixins from <paramref name="configuration"/> mixed in.</returns>
    /// <remarks>This is mostly for internal reasons, users should use <see cref="TypeFactory.GetConcreteType"/> instead.</remarks>
    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (
          configuration,
          delegate (ClassDefinition classConfiguration)
          {
            lock (_scopeLockObject)
            {
              ITypeGenerator generator = Scope.CreateTypeGenerator ((BaseClassDefinition) classConfiguration);
              Type finishedType = generator.GetBuiltType();
              return finishedType;
            }
          });
    }

    /// <summary>
    /// Gets the concrete type for the given mixin class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="configuration">The configuration object for the mixin class.</param>
    /// <returns>A concrete type for the given mixin <paramref name="configuration"/>.</returns>
    /// <remarks>This is mostly for internal reasons, users will hardly ever need to use this method..</remarks>
    public Type GetConcreteMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (
          configuration,
          delegate (ClassDefinition classConfiguration)
          {
            lock (_scopeLockObject)
            {
              IMixinTypeGenerator generator = Scope.CreateMixinTypeGenerator ((MixinDefinition) classConfiguration);
              Type finishedType = generator.GetBuiltType();
              return finishedType;
            }
          });
    }
  }
}
