using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Utilities;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Constitutes a mixin configuration (ie. a set of classes associated with mixins) and manages the mixin configuration for the
  /// current thread.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The purpose of the <see cref="MixinConfiguration"/> class is twofold. First, it holds a thread-local (or, actually,
  /// <see cref="CallContext">CallContext-local</see>) mixin configuration object for the current thread and allows that object to be changed,
  /// see <see cref="MixinConfiguration.ActiveConfiguration"/> and <see cref="ScopedReplace"/> (as well as a number of other Scoped... methods).
  /// Second, each of its instances represent a single mixin configuration, ie. a set of classes associated with mixins.
  /// </para>
  /// <para>
  /// While the <see cref="MixinConfiguration.ActiveConfiguration"/> will usually be accessed only indirectly via <see cref="ObjectFactory"/> or <see cref="TypeFactory"/>,
  /// the configuration replacement mechanism can be of use very often - whenever a mixin configuration needs to be adapted at runtime.
  /// </para>
  /// <para>
  /// The default mixin configuration - the configuration in effect if not specifically replaced by another configuration - is obtained by analyzing
  /// the assemblies referenced in the current AppDomain for attributes such as <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and
  /// <see cref="CompleteInterfaceAttribute"/>. For more information about the default configuration, see <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
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
  /// <threadsafety static="true" instance="false">
  ///    <para>Instances of this class are meant to be used one-per-thread, see <see cref="ActiveConfiguration"/>.</para>
  /// </threadsafety>
  public class MixinConfiguration
  {
    private static readonly CallContextSingleton<MixinConfiguration> _ActiveConfiguration =
    new CallContextSingleton<MixinConfiguration> ("Rubicon.Mixins.MixinConfiguration._ActiveConfiguration",
        delegate { return CopyMasterConfiguration (); });

    private static MixinConfiguration _masterConfiguration = null;
    private static readonly object _masterConfigurationLock = new object ();

    /// <summary>
    /// Gets a value indicating whether this thread has an active mixin configuration.
    /// </summary>
    /// <value>
    ///   True if there is an active configuration for the current thread (actually <see cref="CallContext"/>); otherwise, false.
    /// </value>
    /// <remarks>
    /// The <see cref="ActiveConfiguration"/> property will always return a non-<see langword="null"/> configuration, no matter whether one was
    /// set for the current thread or not. If none was set, a default one is built using <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// In order to check whether a configuration has been set (or a default one has been built), use this property.
    /// </remarks>
    public static bool HasActiveConfiguration
    {
      get { return _ActiveConfiguration.HasCurrent; }
    }

    /// <summary>
    /// Gets the active mixin configuration for the current thread (actually <see cref="CallContext"/>).
    /// </summary>
    /// <value>The active mixin configuration for the current thread (<see cref="CallContext"/>).</value>
    /// <remarks>
    /// The <see cref="ActiveConfiguration"/> property will always return a non-<see langword="null"/> configuration, no matter whether one was
    /// set for the current thread or not. If none was set, a default one is built using <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// In order to check whether a configuration has been set (or a default one has been built), use the <see cref="HasActiveConfiguration"/> property.
    /// </remarks>
    public static MixinConfiguration ActiveConfiguration
    {
      get { return _ActiveConfiguration.Current; }
    }

    private static MixinConfiguration PeekActiveConfiguration
    {
      get { return HasActiveConfiguration ? ActiveConfiguration : null; }
    }

    /// <summary>
    /// Sets the active mixin configuration configuration for the current thread.
    /// </summary>
    /// <param name="configuration">The configuration to be set, can be <see langword="null"/>.</param>
    public static void SetActiveConfiguration (MixinConfiguration configuration)
    {
      _ActiveConfiguration.SetCurrent (configuration);
    }

    /// <summary>
    /// Temporarily replaces the mixin configuration associated with the current thread (actually <see cref="CallContext"/>) with the given
    /// <see cref="MixinConfiguration"/>. The original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method
    /// is called.
    /// </summary>
    /// <param name="newActiveConfiguration">The new active configuration.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="newActiveConfiguration"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedReplace (MixinConfiguration newActiveConfiguration)
    {
      ArgumentUtility.CheckNotNull ("newActiveConfiguration", newActiveConfiguration);

      MixinConfigurationScope scope = new MixinConfigurationScope (PeekActiveConfiguration);
      SetActiveConfiguration (newActiveConfiguration);
      return scope;
    }

    /// <summary>
    /// Creates a new, empty mixin configuration and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <remarks>
    /// This is equivalent to using the <see cref="ScopedReplace"/> method and passing it an empty
    /// <see cref="MixinConfiguration"/> that does not inherit anything from a parent configuration.
    /// </remarks>
    public static IDisposable ScopedEmpty ()
    {
      return ScopedReplace (new MixinConfiguration (null));
    }

    /// <summary>
    /// Creates an <see cref="MixinConfiguration"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created configuration inherits
    /// from the current <see cref="ActiveConfiguration"/> and associates the given <paramref name="baseType"/> with the given <paramref name="mixinTypes"/>
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
    /// Creates an <see cref="MixinConfiguration"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created configuration inherits
    /// from the current <see cref="ActiveConfiguration"/> and includes the given <paramref name="classContexts"/> (overriding any existing configuration
    /// for the same types).
    /// </summary>
    /// <param name="classContexts">The class contexts to be included in the mixin configuration.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="classContexts"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedExtend (params ClassContext[] classContexts)
    {
      ArgumentUtility.CheckNotNull ("classContexts", classContexts);

      MixinConfiguration newConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (ActiveConfiguration, classContexts);
      return ScopedReplace (newConfiguration);
    }

    /// <summary>
    /// Creates an <see cref="MixinConfiguration"/> and temporarily associates it with the current thread (actually <see cref="CallContext"/>). The
    /// original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> method is called. The created configuration inherits
    /// from the current <see cref="ActiveConfiguration"/> and includes all mixin configuration declaratively specified in the given list of
    /// <paramref name="assemblies"/>.
    /// </summary>
    /// <param name="assemblies">The assemblies to be analyzed into the mixin configuration.</param>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static IDisposable ScopedExtend (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      MixinConfiguration newConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (ActiveConfiguration, assemblies);
      return ScopedReplace (newConfiguration);
    }

    private static MixinConfiguration GetMasterConfiguration ()
    {
      lock (_masterConfigurationLock)
      {
        if (_masterConfiguration == null)
          _masterConfiguration = DeclarativeConfigurationBuilder.BuildDefaultConfiguration ();
        return _masterConfiguration;
      }
    }

    private static MixinConfiguration CopyMasterConfiguration ()
    {
      lock (_masterConfigurationLock)
      {
        MixinConfiguration masterConfiguration = GetMasterConfiguration ();
        return new MixinConfiguration (masterConfiguration);
      }
    }

    /// <summary>
    /// Locks access to the application's master mixin configuration and accepts a delegate to edit the configuration while it is locked.
    /// </summary>
    /// <param name="editor">A delegate performing changes to the master configuration.</param>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="ActiveConfiguration"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called. If a thread attempts to access its mixin configuration for the first time while
    /// a change is in progress, it will block until until that process has finished (i.e. until <paramref name="editor"/> has returned).
    /// </remarks>
    public static void EditMasterConfiguration (Proc<MixinConfiguration> editor)
    {
      lock (_masterConfigurationLock)
      {
        editor (GetMasterConfiguration ());
      }
    }

    /// <summary>
    /// Causes the master mixin configuration to be rebuilt from scratch the next time a thread accesses its mixin configuration for the first time.
    /// </summary>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="ActiveConfiguration"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    public static void ResetMasterConfiguration ()
    {
      lock (_masterConfigurationLock)
      {
        _masterConfiguration = null;
      }
    }

    private readonly Dictionary<Type, ClassContext> _classContexts;
    private readonly Dictionary<Type, ClassContext> _registeredInterfaces = new Dictionary<Type,ClassContext> ();

    /// <summary>
    /// Initializes a new empty mixin configuarion that does not inherit anything from another configuration.
    /// </summary>
    public MixinConfiguration ()
        : this (null)
    {
    }

    /// <summary>
    /// Initializes a new configuration that inherits from another configuration.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration. The new configuration will inherit all class contexts from its parent configuration. Can be
    /// <see langword="null"/>.</param>
    public MixinConfiguration (MixinConfiguration parentConfiguration)
    {
      _classContexts = new Dictionary<Type, ClassContext>();
      if (parentConfiguration != null)
        parentConfiguration.CopyTo (this);
    }

    /// <summary>
    /// Gets the class context count for this <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <value>The number of class contexts currently stored in this <see cref="MixinConfiguration"/>.</value>
    public int ClassContextCount
    {
      get { return _classContexts.Count; }
    }

    /// <summary>
    /// Gets the class contexts currently stored in this <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <value>The class contexts currently sotred in this configuration.</value>
    public IEnumerable<ClassContext> ClassContexts
    {
      get { return _classContexts.Values; }
    }

    /// <summary>
    /// Adds a new class context to the <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="classContext">The class context to add.</param>
    /// <exception cref="InvalidOperationException">The <see cref="MixinConfiguration"/> already contains a <see cref="ClassContext"/> for the
    /// same <see cref="Type"/>.</exception>"
    public void AddClassContext (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      if (_classContexts.ContainsKey (classContext.Type))
      {
        string message = string.Format ("There is already a class context for type {0}.", classContext.Type.FullName);
        throw new InvalidOperationException (message);
      }
      _classContexts.Add (classContext.Type, classContext);
    }

    /// <summary>
    /// Retrives the class context for a given <see cref="System.Type"/>, scanning the inheritance hierarchy if no context exists exactly for that
    /// type.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to retrieve a class context for.</param>
    /// <returns>The <see cref="ClassContext"/> stored by the <see cref="MixinConfiguration"/> for the given <see cref="Type"/>, or a copy of the
    /// context of its base type (with an adjusted <see cref="Type"/> member), or <see langword="null"/> if no such context has been registered for
    /// the type hierarchy.</returns>
    public ClassContext GetClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext thisContext = GetClassContextNonRecursive (type);
      if (thisContext != null)
        return thisContext;
      
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
      {
        ClassContext definitionContext = GetClassContextNonRecursive (type.GetGenericTypeDefinition());
        if (definitionContext != null)
          return definitionContext.CloneForSpecificType (type);
      }

      if (type.BaseType != null)
      {
        ClassContext baseContext = GetClassContext (type.BaseType);
        if (baseContext != null)
          return baseContext.CloneForSpecificType (type);
      }

      return null;
    }

    /// <summary>
    /// Retrives the class context for exactly the given <see cref="System.Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to retrieve a class context for.</param>
    /// <returns>The <see cref="ClassContext"/> stored by the <see cref="MixinConfiguration"/> for the given <see cref="Type"/>,
    /// or <see langword="null"/> if no such context has been registered.</returns>
    public ClassContext GetClassContextNonRecursive (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (_classContexts.ContainsKey (type))
        return _classContexts[type];
      else
        return null;
    }

    /// <summary>
    /// Checks whether the <see cref="MixinConfiguration"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for.</param>
    /// <returns>True if the <see cref="MixinConfiguration"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>; false otherwise.
    /// </returns>
    public bool ContainsClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (_classContexts.ContainsKey (type))
        return true;
      else if (type.IsGenericType && !type.IsGenericTypeDefinition)
        return ContainsClassContext (type.GetGenericTypeDefinition());
      else
        return false;
    }

    /// <summary>
    /// Retrieves a <see cref="ClassContext"/> for the given <see cref="Type"/>, or creates and adds a new one if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which a <see cref="ClassContext"/> is to be retrieved.</param>
    /// <returns>A <see cref="ClassContext"/> for the given <see cref="Type"/>.</returns>
    public ClassContext GetOrAddClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ClassContext classContext = GetClassContextNonRecursive (type);
      if (classContext == null || !classContext.Type.Equals (type))
      {
        classContext = new ClassContext (type);
        AddClassContext (classContext);
      }
      return classContext;
    }

    /// <summary>
    /// Removes a class context from the <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> whose class context is to be removed.</param>
    /// <returns>True if the <see cref="MixinConfiguration"/> contained a respective class context; false otherwise.</returns>
    public bool RemoveClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ClassContext context = GetClassContextNonRecursive (type);
      if (context != null && context.Type.Equals (type))
      {
        List<Type> interfacesToRemove = new List<Type>();
        foreach (Type registeredInterface in _registeredInterfaces.Keys)
        {
          if (object.ReferenceEquals (ResolveInterface (registeredInterface), context))
            interfacesToRemove.Add (registeredInterface);
        }
        foreach (Type interfaceToRemove in interfacesToRemove)
          _registeredInterfaces.Remove (interfaceToRemove);

        return _classContexts.Remove (context.Type);
      }
      else
        return false;
    }

    /// <summary>
    /// Adds a class context, replacing the existing one if any.
    /// </summary>
    /// <param name="newContext">The class context to add to this <see cref="MixinConfiguration"/>.</param>
    public void AddOrReplaceClassContext (ClassContext newContext)
    {
      ArgumentUtility.CheckNotNull ("newContext", newContext);

      RemoveClassContext (newContext.Type);
      AddClassContext (newContext);
    }

    /// <summary>
    /// Validates the whole configuration.
    /// </summary>
    /// <returns>An <see cref="IValidationLog"/>, which contains information about whether the configuration reresented by this context is valid.</returns>
    /// <remarks>This method retrieves definition items for all the <see cref="ClassContexts"/> known by this configuration and uses the
    /// <see cref="Validator"/> class to validate them. The validation results can be inspected, passed to a <see cref="ValidationException"/>, or
    /// be dumped using the <see cref="ConsoleDumper"/>.</remarks>
    /// <exception cref="NotSupportedException">The <see cref="MixinConfiguration"/> contains a <see cref="ClassContext"/> for a generic type, of
    /// which it cannot make a closed generic type. Because closed types are needed for validation, this <see cref="MixinConfiguration"/>
    /// cannot be validated as a whole. Even in this case, the configuration might still be correct, but validation is deferred to
    /// <see cref="TypeFactory.GetActiveConfiguration(Type)"/>.</exception>
    public IValidationLog Validate()
    {
      List<IVisitableDefinition> definitions = new List<IVisitableDefinition>();
      List<ValidationException> exceptions = new List<ValidationException> ();

      foreach (ClassContext classContext in ActiveConfiguration.ClassContexts)
      {
        Type typeToVerify;
        if (classContext.Type.IsGenericTypeDefinition)
        {
          try
          {
            typeToVerify = GenericTypeInstantiator.EnsureClosedType (classContext.Type);
          }
          catch (NotSupportedException ex)
          {
            string message = string.Format ("The MixinConfiguration contains a ClassContext for the generic type {0}, of which it cannot make a "
                + "closed type. Because closed types are needed for validation, the MixinConfiguration cannot be validated as a whole. The "
                    + "configuration might still be correct, but validation must be deferred to TypeFactory.GetActiveConfiguration.", classContext.Type);
            throw new NotSupportedException (message, ex);
          }
        }
        else
          typeToVerify = classContext.Type;

        try
        {
          definitions.Add (TypeFactory.GetActiveConfiguration (typeToVerify));
        }
        catch (ValidationException exception)
        {
          exceptions.Add (exception);
        }
      }
      DefaultValidationLog log = Validator.Validate (definitions);
      foreach (ValidationException exception in exceptions)
        log.MergeIn (exception.ValidationLog);

      return log;
    }

    /// <summary>
    /// Registers an interface to be associated with the given <see cref="ClassContext"/>. Later calls to <see cref="ResolveInterface"/>
    /// with the given interface type will result in the registered context being returned.
    /// </summary>
    /// <param name="interfaceType">Type of the interface to be registered.</param>
    /// <param name="associatedClassContext">The class context to be associated with the interface type.</param>
    /// <exception cref="InvalidOperationException">The interface has already been registered.</exception>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface or
    /// <paramref name="associatedClassContext"/> has not been added to this configuration.</exception>
    public void RegisterInterface (Type interfaceType, ClassContext associatedClassContext)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassContext", associatedClassContext);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (!_classContexts.ContainsKey (associatedClassContext.Type)
          || !object.ReferenceEquals (_classContexts[associatedClassContext.Type], associatedClassContext))
        throw new ArgumentException ("The class context hasn't been added to this configuration.", "associatedClassContext");

      if (_registeredInterfaces.ContainsKey (interfaceType))
      {
        string message = string.Format ("The interface {0} has already been associated with a class context.", interfaceType.FullName);
        throw new InvalidOperationException (message);
      }

      _registeredInterfaces.Add (interfaceType, associatedClassContext);
    }

    /// <summary>
    /// Registers an interface to be associated with the <see cref="ClassContext"/> for the given type. Later calls to <see cref="ResolveInterface"/>
    /// with the given interface type will result in the registered context being returned.
    /// </summary>
    /// <param name="interfaceType">Type of the interface to be registered.</param>
    /// <param name="associatedClassType">The type whose class context is to be associated with the interface type.</param>
    /// <exception cref="InvalidOperationException">The interface has already been registered.</exception>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface or no <see cref="ClassContext"/> for
    /// <paramref name="associatedClassType"/> has been added to this configuration.</exception>
    public void RegisterInterface (Type interfaceType, Type associatedClassType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassType", associatedClassType);

      ClassContext context = GetClassContextNonRecursive (associatedClassType);
      if (context == null)
      {
        string message = string.Format ("There is no class context for the given type {0}.", associatedClassType.FullName);
        throw new ArgumentException (message, "associatedClassType");
      }
      else
        RegisterInterface (interfaceType, context);
    }


    /// <summary>
    /// Resolves the given interface into a class context.
    /// </summary>
    /// <param name="interfaceType">The interface type to be resolved.</param>
    /// <returns>The <see cref="ClassContext"/> previously registered for the given type, or <see langword="null"/> if the no context was registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface.</exception>
    public ClassContext ResolveInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (_registeredInterfaces.ContainsKey (interfaceType))
        return _registeredInterfaces[interfaceType];
      else
        return null;
    }

    /// <summary>
    /// Copies all configuration data of this configuration to a destination object, replacing class contexts and registered interfaces
    /// for types that are configured in both objects.
    /// </summary>
    /// <param name="destination">The destination to copy all configuration data to..</param>
    public void CopyTo (MixinConfiguration destination)
    {
      ArgumentUtility.CheckNotNull ("destination", destination);

      foreach (ClassContext classContext in ClassContexts)
        destination.AddOrReplaceClassContext (classContext.Clone());

      foreach (KeyValuePair<Type, ClassContext> interfaceRegistration in _registeredInterfaces)
      {
        if (destination._registeredInterfaces.ContainsKey (interfaceRegistration.Key))
          destination._registeredInterfaces.Remove (interfaceRegistration.Key);
        destination.RegisterInterface (interfaceRegistration.Key, interfaceRegistration.Value.Type);
      }
    }
  }
}