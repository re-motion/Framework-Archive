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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Implements caching of the types generated by <see cref="IModuleManager"/> instances, triggered by <see cref="ConcreteTypeBuilder"/>.
  /// </summary>
  public class CodeGenerationCache
  {
    private sealed class CtorLookupInfoKey
    {
      private readonly ClassContext _classContext;
      private readonly bool _allowNonPublic;

      public CtorLookupInfoKey (ClassContext classContext, bool allowNonPublic)
      {
        _classContext = classContext;
        _allowNonPublic = allowNonPublic;
      }

      public override int GetHashCode ()
      {
        return _classContext.GetHashCode () ^ _allowNonPublic.GetHashCode ();
      }

      public override bool Equals (object obj)
      {
        var other = (CtorLookupInfoKey) obj;
        return other._allowNonPublic == _allowNonPublic && other._classContext.Equals (_classContext);
      }
    }


    private static readonly ILog s_log = LogManager.GetLogger (typeof (CodeGenerationCache));
    
    private readonly object _lockObject = new object();
    private readonly Cache<ClassContext, Type> _typeCache = new Cache<ClassContext, Type> ();
    private readonly Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> _mixinTypeCache = 
        new Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> ();

    private readonly LockingCacheDecorator<CtorLookupInfoKey, IConstructorLookupInfo> _constructorLookupInfos =
        CacheFactory.CreateWithLocking<CtorLookupInfoKey, IConstructorLookupInfo>();

    public void Clear()
    {
      lock (_lockObject)
      {
        _typeCache.Clear();
        _mixinTypeCache.Clear();
        _constructorLookupInfos.Clear();
      }
    }

    public Type GetOrCreateConcreteType (
        IModuleManager moduleManager, 
        ClassContext classContext,
        IConcreteMixedTypeNameProvider nameProvider,
        IConcreteMixinTypeProvider concreteMixinTypeProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("concreteMixinTypeProvider", concreteMixinTypeProvider);

      lock (_lockObject)
      {
        // Use TryGetValue before GetOrCreateValue to avoid creation of closure.
        Type result;
        if (!_typeCache.TryGetValue (classContext, out result))
        {
          result = _typeCache.GetOrCreateValue (
              classContext,
              key => GenerateConcreteType (moduleManager, key, nameProvider, concreteMixinTypeProvider));
        }
        return result;
      }
    }

    private Type GenerateConcreteType (
        IModuleManager moduleManager, 
        ClassContext classContext, 
        IConcreteMixedTypeNameProvider nameProvider, 
        IConcreteMixinTypeProvider concreteMixinTypeProvider)
    {
      s_log.InfoFormat ("Generating concrete type for {0}.", classContext);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to generate concrete type: {elapsed}."))
      using (new CodeGenerationTimer ())
      {
        var targetClassDefinition = TargetClassDefinitionFactory.CreateAndValidate (classContext);
        var generator = moduleManager.CreateTypeGenerator (targetClassDefinition, nameProvider, concreteMixinTypeProvider);

        return generator.GetBuiltType();
      }
    }

    public IConstructorLookupInfo GetOrCreateConstructorLookupInfo (
        IModuleManager manager,
        ClassContext classContext,
        IConcreteMixedTypeNameProvider nameProvider,
        IConcreteMixinTypeProvider concreteMixinTypeProvider,
        bool allowNonPublic)
    {
      ArgumentUtility.CheckNotNull ("manager", manager);
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("concreteMixinTypeProvider", concreteMixinTypeProvider);

      lock (_lockObject)
      {
        var key = new CtorLookupInfoKey (classContext, allowNonPublic);

        // Use TryGetValue before GetOrCreateValue to avoid creation of closure.
        IConstructorLookupInfo result;
        if (!_constructorLookupInfos.TryGetValue (key, out result))
        {
          result = _constructorLookupInfos.GetOrCreateValue (key, cc =>
          {
            var concreteType = GetOrCreateConcreteType (manager, classContext, nameProvider, concreteMixinTypeProvider);
            return new MixedTypeConstructorLookupInfo (concreteType, classContext.Type, allowNonPublic);
          });
        }
        return result;
      }
    }

    public ConcreteMixinType GetOrCreateConcreteMixinType (
        IModuleManager moduleManager,
        ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier,
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        // Use TryGetValue before GetOrCreateValue to avoid creation of closure.
        ConcreteMixinType result;
        if (!_mixinTypeCache.TryGetValue (concreteMixinTypeIdentifier, out result))
        {
          result = _mixinTypeCache.GetOrCreateValue (
              concreteMixinTypeIdentifier,
              key => GenerateConcreteMixinType (moduleManager, concreteMixinTypeIdentifier, mixinNameProvider));
        }
        return result;
      }
    }

    private ConcreteMixinType GenerateConcreteMixinType (
        IModuleManager moduleManager,
        ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier,
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      s_log.InfoFormat ("Generating concrete mixin type for {0}.", concreteMixinTypeIdentifier.MixinType);
      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to generate concrete mixin type: {elapsed}."))
      {
        return moduleManager.CreateMixinTypeGenerator (concreteMixinTypeIdentifier, mixinNameProvider).GetBuiltType ();
      }
    }

    public void ImportTypes (IEnumerable<Type> types, IConcreteTypeMetadataImporter metadataImporter)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      s_log.InfoFormat ("Importing types...");

      lock (_lockObject)
      {
        using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to import types: {elapsed}."))
        {
          foreach (Type type in types)
          {
            ImportConcreteMixedType (metadataImporter, type);
            ImportConcreteMixinType (metadataImporter, type);
          }
        }
      }
    }

    private void ImportConcreteMixedType(IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      var mixedTypeMetadata = metadataImporter.GetMetadataForMixedType (type);
      if (mixedTypeMetadata != null)
      {
        _typeCache.GetOrCreateValue (mixedTypeMetadata, delegate { return type; });
      }
    }

    private void ImportConcreteMixinType (IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      var concreteMixinType = metadataImporter.GetMetadataForMixinType (type);
      if (concreteMixinType != null)
      {
        _mixinTypeCache.GetOrCreateValue (concreteMixinType.Identifier, delegate { return concreteMixinType; });
      }
    }
  }
}
