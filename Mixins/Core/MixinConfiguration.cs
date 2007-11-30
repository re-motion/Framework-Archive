using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Manages the mixin configuration for the current thread.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The purpose of the <see cref="MixinConfiguration"/> class is twofold: first, it holds a thread-local (or, actually,
  /// <see cref="CallContext">CallContext-local</see>) mixin configuration object for the current thread, <see cref="ActiveContext"/>;
  /// second, it provides a mechanism to replace the current mixin configuration by a new configuration and to restore the original configuration
  /// at a later point of time through its constructors and <see cref="IDisposable"/> implementation.
  /// </para>
  /// <para>
  /// While the <see cref="ActiveContext"/> will usually be accessed only indirectly via <see cref="ObjectFactory"/> or <see cref="TypeFactory"/>,
  /// the configuration replacement mechanism can be of use very often - whenever a mixin configuration needs to be adapted at runtime.
  /// </para>
  /// <para>
  /// The default mixin configuration - the configuration in effect if not specifically replaced by another configuration - is obtained by analyzing
  /// the assemblies referenced in the current AppDomain for attributes such as <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and
  /// <see cref="CompleteInterfaceAttribute"/>. For more information about the default configuration, see <see cref="ApplicationContextBuilder.BuildDefaultContext"/>.
  /// </para>
  /// <example>
  /// The following shows an exemplary application of the <see cref="MixinConfiguration"/> class.
  /// <code>
  /// class Program
  /// {
  ///   public static void Main()
  ///   {
  ///     // myType1 is an instantiation of MyType with the default mixin configuration
  ///     MyType myType1 = ObjectFactory.Create&lt;MyType&gt; ().With();
  /// 
  ///     using (MixinConfiguration.ScopedExtend (typeof (MyType), typeof (SpecialMixin)))
  ///     {
  ///       // myType2 is an instantiation of MyType with a specific configuration, which contains only SpecialMixin
  ///       MyType myType2 = ObjectFactory.Create&lt;MyType&gt; ().With();
  /// 
  ///       using (MixinConfiguration.ScopedEmpty())
  ///       {
  ///         // myType3 is an instantiation of MyType without any mixins
  ///         MyType myType3 = ObjectFactory.Create&lt;MyType&gt; ().With();
  ///       }
  ///     }
  /// 
  ///     // myType4 is again an instantiation of MyType with the default mixin configuration
  ///     MyType myType4 = ObjectFactory.Create&lt;MyType&gt; ().With();
  ///   }
  /// }
  /// </code>
  /// </example>
  /// </remarks>
  public class MixinConfiguration : IDisposable
  {
    private static readonly CallContextSingleton<ApplicationContext> _activeContext =
        new CallContextSingleton<ApplicationContext> ("Rubicon.Mixins.MixinConfiguration._activeContext",
        delegate { return CopyMasterContext (); });

    private static ApplicationContext _masterContext = null;
    private static readonly object _masterContextLock = new object ();

    /// <summary>
    /// Gets a value indicating whether this instance has an active mixin configuration context.
    /// </summary>
    /// <value>
    ///   True if there is an active configuration context for the current thread (actually <see cref="CallContext"/>); otherwise, false.
    /// </value>
    /// <remarks>
    /// The <see cref="ActiveContext"/> property will always return a non-<see langword="null"/> application context, no matter whether one was
    /// set for the current thread or not. If none was set, a default context is built using <see cref="ApplicationContextBuilder.BuildDefaultContext"/>.
    /// In order to check whether a configuration context has been set (or a default one has been built), use this property.
    /// </remarks>
    public static bool HasActiveContext
    {
      get { return _activeContext.HasCurrent; }
    }

    /// <summary>
    /// Gets the active mixin configuration context for the current thread (actually <see cref="CallContext"/>).
    /// </summary>
    /// <value>The active mixin configuration context for the current thread (<see cref="CallContext"/>).</value>
    /// <remarks>
    /// The <see cref="ActiveContext"/> property will always return a non-<see langword="null"/> application context, no matter whether one was
    /// set for the current thread or not. If none was set, a default context is built using <see cref="ApplicationContextBuilder.BuildDefaultContext"/>.
    /// In order to check whether a configuration context has been set (or a default one has been built), use the <see cref="HasActiveContext"/> property.
    /// </remarks>
    public static ApplicationContext ActiveContext
    {
      get { return _activeContext.Current; }
    }

    /// <summary>
    /// Sets the active mixin configuration context for the current thread.
    /// </summary>
    /// <param name="context">The context to be set, can be <see langword="null"/>.</param>
    public static void SetActiveContext (ApplicationContext context)
    {
      _activeContext.SetCurrent (context);
    }

    private static ApplicationContext PeekActiveContext
    {
      get { return MixinConfiguration.HasActiveContext ? MixinConfiguration.ActiveContext : null; }
    }

    /// <summary>
    /// Temporarily replaces the mixin configuration associated with the current thread (actually <see cref="CallContext"/>) with the given
    /// <see cref="ApplicationContext"/>. The original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method
    /// is called.
    /// </summary>
    /// <param name="newActiveContext">The new active configuration context.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="newActiveContext"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedReplace (ApplicationContext newActiveContext)
    {
      ArgumentUtility.CheckNotNull ("newActiveContext", newActiveContext);

      MixinConfiguration newConfiguration = new MixinConfiguration ();
      MixinConfiguration.SetActiveContext (newActiveContext);
      return newConfiguration;
    }

    /// <summary>
    /// Creates a new, empty mixin configuration and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <remarks>
    /// This is equivalent to using the <see cref="ScopedReplace(ApplicationContext)"/> method and passing it an empty
    /// <see cref="ApplicationContext"/> that does not inherit anything from a parent context.
    /// </remarks>
    public static IDisposable ScopedEmpty ()
    {
      return ScopedReplace (new ApplicationContext (null));
    }

    /// <summary>
    /// Creates an <see cref="ApplicationContext"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created context inherits
    /// from the current <see cref="ActiveContext"/> and associates the given <paramref name="baseType"/> with the given <paramref name="mixinTypes"/>
    /// (replacing the configuration for <paramref name="baseType"/> if any).
    /// </summary>
    /// <param name="baseType">A type for which a specific mixin configuration should be set up.</param>
    /// <param name="mixinTypes">The mixin types to be associated with the <paramref name="baseType"/>.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> or the <paramref name="mixinTypes"/> parameter is
    /// <see langword="null"/>.</exception>
    public static IDisposable ScopedExtend (Type baseType, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      return ScopedExtend (new ClassContext (baseType, mixinTypes));
    }

    /// <summary>
    /// Creates an <see cref="ApplicationContext"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created context inherits
    /// from the current <see cref="ActiveContext"/> and includes the given <paramref name="classContexts"/> (overriding any existing configuration
    /// for the same types).
    /// </summary>
    /// <param name="classContexts">The class contexts to be included in the mixin configuration.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="classContexts"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedExtend (params ClassContext[] classContexts)
    {
      ArgumentUtility.CheckNotNull ("classContexts", classContexts);

      ApplicationContext newContext = ApplicationContextBuilder.BuildContextFromClasses (MixinConfiguration.ActiveContext, classContexts);
      return ScopedReplace (newContext);
    }

    /// <summary>
    /// Creates an <see cref="ApplicationContext"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created context inherits
    /// from the current <see cref="ActiveContext"/> and includes all mixin configuration declaratively specified in the given list of
    /// <paramref name="assemblies"/>.
    /// </summary>
    /// <param name="assemblies">The assemblies to be analyzed into the mixin configuration.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedExtend (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      ApplicationContext newContext = ApplicationContextBuilder.BuildContextFromAssemblies (MixinConfiguration.ActiveContext, assemblies);
      return ScopedReplace (newContext);
    }

    private static ApplicationContext GetMasterContext()
    {
      lock (_masterContextLock)
      {
        if (_masterContext == null)
          _masterContext = ApplicationContextBuilder.BuildDefaultContext();
        return _masterContext;
      }
    }

    private static ApplicationContext CopyMasterContext ()
    {
      lock (_masterContextLock)
      {
        ApplicationContext masterContext = GetMasterContext ();
        return new ApplicationContext (masterContext);
      }
    }

    /// <summary>
    /// Locks access to the application's master mixin configuration and accepts a delegate to edit the configuration while it is locked.
    /// </summary>
    /// <param name="editor">A delegate performing changes to the master configuration.</param>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="MixinConfiguration.ActiveContext"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called. If a thread attempts to access its mixin configuration for the first time while
    /// a change is in progress, it will block until until that process has finished (i.e. until <paramref name="editor"/> has returned).
    /// </remarks>
    public static void EditMasterConfiguration (Proc<ApplicationContext> editor)
    {
      lock (_masterContextLock)
      {
        editor (GetMasterContext ());
      }
    }

    /// <summary>
    /// Causes the master mixin configuration to be rebuilt from scratch the next time a thread accesses its mixin configuration for the first time.
    /// </summary>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="MixinConfiguration.ActiveContext"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    public static void ResetMasterConfiguration ()
    {
      lock (_masterContextLock)
      {
        _masterContext = null;
      }
    }

    private readonly ApplicationContext _previousContext = null;
    private bool _disposed;

    /// <summary>
    /// Creates a mixin configuration object which resets the current <see cref="ActiveContext"/> when its <see cref="IDisposable.Dispose"/> method is called.
    /// </summary>
    private MixinConfiguration ()
    {
      _previousContext = MixinConfiguration.PeekActiveContext;
    }

    /// <summary>
    /// When called for the first time, restores the <see cref="ApplicationContext"/> that was the <see cref="ActiveContext"/> for the current
    /// thread (<see cref="CallContext"/>) before this object was constructed.
    /// </summary>
    /// <remarks>
    /// After this method has been called for the first time, further calls have no effects. If the <see cref="IDisposable.Dispose"/> method is not called, the
    /// original configuration will not be restored by this object.
    /// </remarks>
    void IDisposable.Dispose ()
    {
      if (!_disposed)
      {
        MixinConfiguration.SetActiveContext (_previousContext);
        _disposed = true;
      }
    }
  }
}
