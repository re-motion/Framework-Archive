using System;
using System.Runtime.Remoting.Messaging;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Utilities.Singleton;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Allows users to specify configuration settings when a mixed type is instantiated.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
  /// instance is constructed. Using this scope class, a user can supply pre-instantiated mixins instead.
  /// </para>
  /// <para>
  /// This is mainly for internal purposes, users should use the <see cref="ObjectFactory"/>
  /// class to instantiate mixed types.
  /// </para>
  /// <para>
  /// This class is a singleton bound to the current <see cref="CallContext"/>.
  /// </para>
  /// </remarks>
  public class MixedTypeInstantiationScope
      : CallContextSingletonBase<MixedTypeInstantiationScope, DefaultInstanceCreator<MixedTypeInstantiationScope>>, IDisposable
  {
    /// <summary>
    /// The mixin instances to be used when a mixed class is instantiated from within the scope.
    /// </summary>
    public readonly object[] SuppliedMixinInstances;

    private MixedTypeInstantiationScope _previous;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedTypeInstantiationScope"/> class, setting it as the
    /// <see cref="CallContextSingletonBase{TSelf,TCreator}.Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope will not contain any pre-created mixin instances.
    /// </summary>
    public MixedTypeInstantiationScope ()
    {
      StorePreviousAndSetCurrent ();
      SuppliedMixinInstances = new object[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedTypeInstantiationScope"/> class, setting it as the
    /// <see cref="CallContextSingletonBase{TSelf,TCreator}.Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope contains the specified pre-created mixin instances.
    /// </summary>
    /// <param name="suppliedMixinInstances">The mixin instances to be used when a mixed type is instantiated from within the scope. The objects
    /// specified must fit the mixin types specified in the mixed type's configuration. Users can also specify instances for a subset of the mixin
    /// types, the remaining ones will be created on demand.</param>
    public MixedTypeInstantiationScope (params object[] suppliedMixinInstances)
    {
      StorePreviousAndSetCurrent ();
      SuppliedMixinInstances = suppliedMixinInstances;
    }

    /// <summary>
    /// When called for the first time, restores the <see cref="MixedTypeInstantiationScope"/> that was in effect when this scope was created.
    /// </summary>
    public void Dispose ()
    {
      if (!_disposed)
      {
        RestorePrevious ();
        _disposed = true;
      }
    }

    private void StorePreviousAndSetCurrent ()
    {
      if (MixedTypeInstantiationScope.HasCurrent)
        _previous = MixedTypeInstantiationScope.Current;
      else
        _previous = null;

      MixedTypeInstantiationScope.SetCurrent (this);
    }

    private void RestorePrevious ()
    {
      MixedTypeInstantiationScope.SetCurrent (_previous);
    }
  }
}
