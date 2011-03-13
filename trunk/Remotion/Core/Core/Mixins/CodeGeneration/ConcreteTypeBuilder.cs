// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities.Singleton;
using Remotion.Reflection;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Mixins.Context;
using Remotion.Logging;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a way to build concrete types for target and mixin classes in mixin configurations and maintains a cache for built types.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You can use different instances of <see cref="ConcreteTypeBuilder"/> by setting them as the
  /// <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.Current"/> property via <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.SetCurrent"/>.
  /// Each of these will have its own instance of <see cref="IModuleManager"/> as its <see cref="Scope"/>, and each scope will have its own
  /// dynamic assembly for code generation. However, each scope's assembly will have the same default name. 
  /// </para>
  /// <para>
  /// Having different assemblies with the same names loaded into one AppDomain can lead to sporadic
  /// <see cref="TypeLoadException">TypeLoadExceptions</see> in reflective scenarios. To avoid running into such errors, set the 
  /// <see cref="IModuleManager.SignedAssemblyName"/> and <see cref="IModuleManager.UnsignedAssemblyName"/> properties of the
  /// <see cref="Scope"/> to unique names for each <see cref="ConcreteTypeBuilder"/> instance you use.
  /// </para>
  /// </remarks>
  public class ConcreteTypeBuilder : ThreadSafeSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>, IConcreteTypeBuilder
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ConcreteTypeBuilder));

    private readonly CodeGenerationCache _cache;

    private readonly object _scopeLockObject = new object ();
    private IModuleManager _scope;

    private IConcreteMixedTypeNameProvider _typeNameProvider = GuidNameProvider.Instance;
    private IConcreteMixinTypeNameProvider _mixinTypeNameProvider = GuidNameProvider.Instance;

    public ConcreteTypeBuilder ()
    {
      _cache = new CodeGenerationCache(this);
    }

    /// <summary>
    /// Gets the cache used to cache types generated by this <see cref="ConcreteTypeBuilder"/>.
    /// </summary>
    /// <value>The cache used for building types.</value>
    public CodeGenerationCache Cache
    {
      get { return _cache; }
    }

    /// <summary>
    /// Gets or sets the module scope of this <see cref="ConcreteTypeBuilder"/>. The object returned by this property must not be used by multiple
    /// threads at the same time (or while another thread executes methods on <see cref="ConcreteTypeBuilder"/>). Use the
    /// <see cref="LockAndAccessScope"/> method to access the scope in a thread-safe way.
    /// </summary>
    /// <value>The module scope of this <see cref="ConcreteTypeBuilder"/>. If set to <see langword="null"/>, a new <see cref="IModuleManager"/>
    /// will be created the next time the <see cref="Scope"/> property is accessed.</value>
    public IModuleManager Scope
    {
      get
      {
        lock (_scopeLockObject)
        {
          if (_scope == null)
            _scope = new ModuleManager();
          return _scope;
        }
      }
      set
      {
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
    public void LockAndAccessScope (Action<IModuleManager> scopeAccessor)
    {
      lock (_scopeLockObject)
      {
        scopeAccessor (Scope);
      }
    }

    /// <summary>
    /// Gets or sets the name provider used when generating a concrete mixed type.
    /// </summary>
    /// <value>The type name provider for mixed types.</value>
    public IConcreteMixedTypeNameProvider TypeNameProvider
    {
      get
      {
        lock (_scopeLockObject)
        {
          return _typeNameProvider;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _typeNameProvider = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the name provider used when generating a concrete mixin type.
    /// </summary>
    /// <value>The type name provider for mixin types.</value>
    public IConcreteMixinTypeNameProvider MixinTypeNameProvider
    {
      get
      {
        lock (_scopeLockObject)
        {
          return _mixinTypeNameProvider;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _mixinTypeNameProvider = value;
        }
      }
    }

    /// <summary>
    /// Gets the concrete mixed type for the given target class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="classContext">The <see cref="ClassContext"/> holding the mixin configuration for the target class.</param>
    /// <returns>A concrete type with all mixins from <paramref name="classContext"/> mixed in.</returns>
    /// <remarks>This is mostly for internal reasons, users should use <see cref="TypeFactory.GetConcreteType(Type)"/> instead.</remarks>
    public Type GetConcreteType (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      return Cache.GetOrCreateConcreteType (Scope, classContext, _typeNameProvider, _mixinTypeNameProvider);
    }

    /// <summary>
    /// Gets an <see cref="IConstructorLookupInfo"/> object that can be used to construct the concrete mixed type for the given target class
    /// configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="classContext">The <see cref="ClassContext"/> holding the mixin configuration for the target class.</param>
    /// <param name="allowNonPublic">If set to <see langword="true"/>, the result object supports calling non-public constructors. Otherwise,
    /// only public constructors are allowed.</param>
    /// <returns>
    /// An <see cref="IConstructorLookupInfo"/> instance instantiating the same type <see cref="GetConcreteType"/> would have returned for the given
    /// <paramref name="classContext"/>.
    /// </returns>
    /// <remarks>
    /// This is mostly for internal reasons, users should use <see cref="ObjectFactory.Create(System.Type,Remotion.Reflection.ParamList,object[])"/> 
    /// instead.
    /// </remarks>
    public IConstructorLookupInfo GetConstructorLookupInfo (ClassContext classContext, bool allowNonPublic)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      return Cache.GetOrCreateConstructorLookupInfo (Scope, classContext, _typeNameProvider, _mixinTypeNameProvider, allowNonPublic);
    }

    /// <summary>
    /// Gets the concrete type for the given mixin class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="concreteMixinTypeIdentifier">The <see cref="ConcreteMixinTypeIdentifier"/> defining the mixin type to get.</param>
    /// <returns>A concrete type for the given <paramref name="concreteMixinTypeIdentifier"/>.</returns>
    /// <remarks>This is mostly for internal reasons, users will hardly ever need to use this method.</remarks>
    public ConcreteMixinType GetConcreteMixinType (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);
     
      ConcreteMixinType concreteMixinType = Cache.GetOrCreateConcreteMixinType (concreteMixinTypeIdentifier, _mixinTypeNameProvider);
      return concreteMixinType;
    }

    /// <summary>
    /// Saves the dynamic <see cref="Scope"/> of this builder to disk and resets it, so that the builder can continue to generate types. Use
    /// the <see cref="Scope">Scope's</see> properties via <see cref="LockAndAccessScope"/> to configure the name and paths of the modules being
    /// saved.
    /// </summary>
    /// <returns>An array containing the paths of the assembly files saved.</returns>
    /// <remarks>
    /// <para>
    /// This is similar to directly calling <c>Scope.SaveAssemblies</c>, but in addition resets the <see cref="Scope"/> to a new instance of
    /// <see cref="IModuleManager"/>. That way, the builder can continue to generate types even when the dynamic assemblies have been saved.
    /// Note that each time this method is called, only the types generated since the last save operation are persisted. Also, if the scope isn't
    /// reconfigured to save at different paths, previously saved assemblies might be overwritten.
    /// </para>
    /// <para>
    /// Having different assemblies with the same names loaded into one AppDomain can lead to sporadic
    /// <see cref="TypeLoadException">TypeLoadExceptions</see> in reflective scenarios. To avoid running into such errors, set the 
    /// <see cref="IModuleManager.SignedAssemblyName"/> and <see cref="IModuleManager.UnsignedAssemblyName"/> properties of the
    /// <see cref="Scope"/> to new, unique names after calling this method.
    /// </para>
    /// </remarks>
    public string[] SaveAndResetDynamicScope ()
    {
      s_log.Info ("Saving built types...");
      lock (_scopeLockObject)
      {
        string[] paths;
        if (Scope.HasSignedAssembly || Scope.HasUnsignedAssembly)
          paths = Scope.SaveAssemblies();
        else
          paths = new string[0];

        _scope = null;
        return paths.LogAndReturn (s_log, LogLevel.Info, result => "Saved files: " + SeparatedStringBuilder.Build (", ", result));
      }
    }

    /// <summary>
    /// Loads an assembly with the given name and adds its mixed types to this builder's cache.
    /// </summary>
    /// <param name="assembly">The assembly whose public types to load into the cache.</param>
    public void LoadAssemblyIntoCache (Assembly assembly)
    {
      LoadAssemblyIntoCache ((_Assembly) assembly);
    }

    /// <summary>
    /// Loads an assembly with the given name and adds its mixed types to this builder's cache.
    /// </summary>
    /// <param name="assembly">The assembly whose public types to load into the cache.</param>
    /// <remarks>
    /// This overload exists primarily for testing purposes; it has the same functionality as <see cref="LoadAssemblyIntoCache(Assembly)"/>.
    /// </remarks>
    [CLSCompliant (false)]
    public void LoadAssemblyIntoCache (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      s_log.InfoFormat ("Loading assembly {0} into cache...", assembly);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to load assembly into cache: {elapsed}."))
      {
        AssemblyName assemblyName = assembly.GetName();
        if (assemblyName.Name == Scope.SignedAssemblyName || assemblyName.Name == Scope.UnsignedAssemblyName)
        {
          string message = string.Format (
              "Cannot load assembly '{0}' into the cache because it has the same name as one of the dynamic assemblies used "
              + "by the mixin engine. Having two assemblies with the same name loaded into one AppDomain can cause strange and sporadic "
              + "TypeLoadExceptions.",
              assemblyName.Name);
          throw new ArgumentException (message, "assembly");
        }

        var types = assembly.GetExportedTypes();
        s_log.InfoFormat ("Assembly {0} has {1} exported types.", types.Length);
        Cache.ImportTypes (types, new AttributeBasedMetadataImporter());
      }
    }

    /// <summary>
    /// Initializes a mixin target instance which was created without its constructor having been called.
    /// </summary>
    /// <param name="mixinTarget">The mixin target to initialize.</param>
    /// <exception cref="ArgumentNullException">The mixin target is <see langword="null"/>.</exception>
    /// <remarks>This method is useful when a mixin target instance is created via <see cref="FormatterServices.GetSafeUninitializedObject"/>.</remarks>
    public void InitializeUnconstructedInstance (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      Scope.InitializeMixinTarget (mixinTarget);
    }

    /// <summary>
    /// Begins deserialization of a mixed object.
    /// </summary>
    /// <param name="typeTransformer">A transformation object that is given a chance to modify the deserialized type before it is instantiated.</param>
    /// <param name="info">The <see cref="SerializationInfo"/> object provided by the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> object provided by the .NET serialization infrastructure.</param>
    /// <returns>An <see cref="IObjectReference"/> object containing a partially deserialized mixed object. Be sure to call
    /// <see cref="FinishDeserialization"/> from an implementation of <see cref="IDeserializationCallback.OnDeserialization"/> to finish the
    /// deserialization process.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this method are <see langword="null"/>.</exception>
    /// <exception cref="SerializationException">The serialization data does not hold the expected values.</exception>
    /// <remarks>
    /// <para>
    /// This method is useful when the mixin engine is combined with other code generation mechanisms. In such a case, the default
    /// <see cref="IObjectReference"/> implementation provided by the mixin code generation can be extended by a custom <see cref="IObjectReference"/>
    /// object by calling this method. This method instantiates the real object to be returned by the deserialization process, but the caller
    /// specifies a <paramref name="typeTransformer"/> delegate that gets the chance to modify the type of object before it is instantiated. The
    /// parameter passed to <paramref name="typeTransformer"/> is the type deducted from the deserialized mixin configuration-
    /// </para>
    /// <para>
    /// This method expects that the deserialized data is from a mixed object, calling it for an unmixed object will yield an exception.
    /// </para>
    /// </remarks>
    public IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      return Scope.BeginDeserialization (typeTransformer, info, context);
    }

    /// <summary>
    /// Finishes a deserialization process started by <see cref="BeginDeserialization"/>.
    /// </summary>
    /// <param name="objectReference">The object returned from <see cref="BeginDeserialization"/>.</param>
    /// <remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="objectReference"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="objectReference"/> parameter does not hold an object returned by the
    /// <see cref="BeginDeserialization"/> method.</exception>
    /// <para>
    /// Call this method to complete deserialization of a mixed object when the .NET serialization infrastructure has finished its
    /// work, e.g. from an implementation of <see cref="IDeserializationCallback.OnDeserialization"/>. After this method, the real object
    /// contained in <paramref name="objectReference"/> can safely be used.
    /// </para>
    /// <para>
    /// If the given instance is not an instance of a mixed type, this method does nothing.
    /// </para>
    /// </remarks>
    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      Scope.FinishDeserialization (objectReference);
    }
  }
}
