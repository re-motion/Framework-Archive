// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
  /// This class represents a thread-safe global singleton. You can access the single instance via the 
  /// <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.Current"/> property and replace it via the 
  /// <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.SetCurrent"/> method.
  /// </para>
  /// <para>
  /// The names of the assemblies generated by the <see cref="ConcreteTypeBuilder"/> is determined by the <see cref="IModuleManager"/> it uses.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public class ConcreteTypeBuilder
      : ThreadSafeSingletonBase<IConcreteTypeBuilder, ServiceLocatorInstanceCreator<IConcreteTypeBuilder>>, IConcreteTypeBuilder
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ConcreteTypeBuilder));

    private readonly IModuleManager _moduleManager;
    private readonly IConcreteMixedTypeNameProvider _mixedTypeNameProvider;
    private readonly IConcreteMixinTypeNameProvider _mixinTypeNameProvider;
    private readonly ICodeGenerationCache _threadsafeCache;

    private readonly object _codeGenerationLockObject = new object ();

    public ConcreteTypeBuilder (
        IModuleManager moduleManager,
        IConcreteMixedTypeNameProvider mixedTypeNameProvider,
        IConcreteMixinTypeNameProvider mixinTypeNameProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("mixedTypeNameProvider", mixedTypeNameProvider);
      ArgumentUtility.CheckNotNull ("mixinTypeNameProvider", mixinTypeNameProvider);

      _moduleManager = moduleManager;
      _mixedTypeNameProvider = mixedTypeNameProvider;
      _mixinTypeNameProvider = mixinTypeNameProvider;
      var innerCache = new CodeGenerationCache (_moduleManager, _mixedTypeNameProvider, _mixinTypeNameProvider, this);
      _threadsafeCache = new LockingCodeGenerationCacheDecorator (innerCache, _codeGenerationLockObject);
    }

    /// <summary>
    /// Gets the cache used to cache types generated by this <see cref="ConcreteTypeBuilder"/>.
    /// </summary>
    /// <value>The cache used for building types.</value>
    public ICodeGenerationCache Cache
    {
      get { return _threadsafeCache; }
    }

    /// <summary>
    /// Gets information about, and allows configuration of, the modules used by this <see cref="ConcreteTypeBuilder"/>.
    /// </summary>
    /// <value>An <see cref="ICodeGenerationModuleInfo"/> object for this <see cref="ConcreteTypeBuilder"/>.</value>
    public ICodeGenerationModuleInfo ModuleInfo
    {
      get { return new LockingCodeGenerationModuleInfoDecorator (_moduleManager, _codeGenerationLockObject); }
    }

    /// <summary>
    /// Gets the name provider used when generating a concrete mixed type.
    /// </summary>
    public IConcreteMixedTypeNameProvider TypeNameProvider
    {
      get { return _mixedTypeNameProvider; }
    }

    /// <summary>
    /// Gets the name provider used when generating a concrete mixin type.
    /// </summary>
    public IConcreteMixinTypeNameProvider MixinTypeNameProvider
    {
      get { return _mixinTypeNameProvider; }
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

      return _threadsafeCache.GetOrCreateConcreteType (classContext);
    }

    /// <inheritdoc />
    public IConstructorLookupInfo GetConstructorLookupInfo (ClassContext classContext, bool allowNonPublic)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      return _threadsafeCache.GetOrCreateConstructorLookupInfo (classContext, allowNonPublic);
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

      return _threadsafeCache.GetOrCreateConcreteMixinType (concreteMixinTypeIdentifier);
    }

    /// <summary>
    /// Saves the concrete types and mixin types generated by this <see cref="IConcreteTypeBuilder"/> to disk. The <see cref="ModuleInfo"/> is reset, 
    /// so that the builder can continue to generate types.
    /// The names of the assemblies used by this method depend on the <see cref="ModuleInfo"/>. It is recommended to provide a configured 
    /// <see cref="IModuleManager"/> instance when constructing the <see cref="ConcreteTypeBuilder"/> in order to configure the assembly names.
    /// </summary>
    /// <returns>An array containing the paths of the assembly files saved.</returns>
    /// <remarks>
    /// <para>
    /// This method is similar to directly calling <see cref="ModuleInfo"/>.<see cref="IModuleManager.SaveAssemblies"/> , but in addition resets the 
    /// <see cref="ModuleInfo"/> via <see cref="IModuleManager.Reset"/>. That way, the builder can continue to generate types even when the dynamic 
    /// assemblies have been saved. Note that each time this method is called, only the types generated since the last save operation are persisted. 
    /// Also, if the scope isn't reconfigured to save at different paths, previously saved assemblies might be overwritten.
    /// </para>
    /// </remarks>
    public string[] SaveGeneratedConcreteTypes ()
    {
      s_log.Info ("Saving built types...");

      lock (_codeGenerationLockObject)
      {
        string[] paths;
        if (_moduleManager.HasSignedAssembly || _moduleManager.HasUnsignedAssembly)
          paths = _moduleManager.SaveAssemblies ();
        else
          paths = new string[0];

        _moduleManager.Reset();
        return paths.LogAndReturn (s_log, LogLevel.Info, result => "Saved files: " + SeparatedStringBuilder.Build (", ", result));
      }
    }

    /// <inheritdoc />
    public void LoadConcreteTypes (Assembly assembly)
    {
      LoadConcreteTypes ((_Assembly) assembly);
    }

    /// <summary>
    /// Imports the public concrete types and mixin types from the given <see cref="Assembly"/> into this <see cref="IConcreteTypeBuilder"/> instance.
    /// </summary>
    /// <param name="assembly">The assembly whose public types to load.</param>
    /// <remarks>
    /// This overload exists primarily for testing purposes; it has the same functionality as <see cref="LoadConcreteTypes(System.Reflection.Assembly)"/>.
    /// </remarks>
    [CLSCompliant (false)]
    public void LoadConcreteTypes (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      s_log.InfoFormat ("Loading assembly {0} into cache...", assembly);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to load assembly into cache: {elapsed}."))
      {
        AssemblyName assemblyName = assembly.GetName();

        // Lock this whole block to ensure that the _moduleManager's name doesn't change until the import has finished.
        lock (_codeGenerationLockObject)
        {
          if (assemblyName.Name == _moduleManager.SignedAssemblyName || assemblyName.Name == _moduleManager.UnsignedAssemblyName)
          {
            string message = string.Format (
                "Cannot load assembly '{0}' into the cache because it has the same name as one of the dynamic assemblies used "
                + "by the mixin engine. Having two assemblies with the same name loaded into one AppDomain can cause strange and sporadic "
                + "TypeLoadExceptions.",
                assemblyName.Name);
            throw new ArgumentException (message, "assembly");
          }

          var types = assembly.GetExportedTypes ();
          s_log.InfoFormat ("Assembly {0} has {1} exported types.", assemblyName, types.Length);
          
          _threadsafeCache.ImportTypes (types, new AttributeBasedMetadataImporter ());
        }
      }
    }

    /// <inheritdoc />
    public void InitializeUnconstructedInstance (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      // No locking needed, this API is thread-safe.
      _moduleManager.InitializeMixinTarget (mixinTarget);
    }

    /// <inheritdoc />
    public void InitializeDeserializedInstance (IMixinTarget mixinTarget, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      // No locking needed, this API is thread-safe.
      _moduleManager.InitializeDeserializedMixinTarget (mixinTarget, mixinInstances);
    }

    /// <inheritdoc />
    public IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      // No locking needed, this API is thread-safe.
      return _moduleManager.BeginDeserialization (typeTransformer, info, context);
    }

    /// <inheritdoc />
    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);

      // No locking needed, this API is thread-safe.
      _moduleManager.FinishDeserialization (objectReference);
    }
  }
}
