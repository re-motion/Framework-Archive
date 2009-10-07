// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Implements caching of the types generated by <see cref="IModuleManager"/> instances, triggered by <see cref="ConcreteTypeBuilder"/>.
  /// </summary>
  public class CodeGenerationCache
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CodeGenerationCache));
    
    private readonly object _lockObject = new object();
    private readonly ConcreteTypeBuilder _concreteTypeBuilder;
    private readonly Cache<ClassContext, Type> _typeCache = new Cache<ClassContext, Type> ();
    private readonly Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> _mixinTypeCache = 
        new Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> ();

    public CodeGenerationCache (ConcreteTypeBuilder concreteTypeBuilder)
    {
      ArgumentUtility.CheckNotNull ("concreteTypeBuilder", concreteTypeBuilder);
      _concreteTypeBuilder = concreteTypeBuilder;
    }

    public Type GetOrCreateConcreteType (
        IModuleManager moduleManager, 
        ClassContext classContext,
        IConcreteMixedTypeNameProvider nameProvider,
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _typeCache.GetOrCreateValue (
            classContext,
            key => GenerateConcreteType (moduleManager, key, nameProvider, mixinNameProvider));
      }
    }

    private Type GenerateConcreteType (
        IModuleManager moduleManager, 
        ClassContext classContext, 
        IConcreteMixedTypeNameProvider nameProvider, 
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      s_log.InfoFormat ("Generating concrete type for {0}.", classContext);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to generate concrete type: {0}."))
      using (new CodeGenerationTimer ())
      {
        var targetClassDefinition = TargetClassDefinitionFactory.CreateTargetClassDefinition (classContext);
        ITypeGenerator generator = moduleManager.CreateTypeGenerator (
            this,
            targetClassDefinition,
            nameProvider,
            mixinNameProvider);

        return generator.GetBuiltType();
      }
    }

    public ConcreteMixinType GetOrCreateConcreteMixinType (
        MixinDefinition mixinDefinition,
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _mixinTypeCache.GetOrCreateValue (
              mixinDefinition.GetConcreteMixinTypeIdentifier (),
              key => GenerateConcreteMixinType (mixinDefinition, mixinNameProvider));
      }
    }

    private ConcreteMixinType GenerateConcreteMixinType (
        MixinDefinition mixinDefinition,
        IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      s_log.InfoFormat ("Generating concrete mixin type for {0}.", mixinDefinition.Type);
      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to generate concrete mixin type: {0}."))
      {
        return _concreteTypeBuilder.Scope.CreateMixinTypeGenerator (mixinDefinition, mixinNameProvider).GetBuiltType();
      }
    }

    public ConcreteMixinType GetConcreteMixinTypeFromCacheOnly (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);
      lock (_lockObject)
      {
        ConcreteMixinType type;
        _mixinTypeCache.TryGetValue (concreteMixinTypeIdentifier, out type);
        return type;
      }
    }

    public void ImportTypes (IEnumerable<Type> types, IConcreteTypeMetadataImporter metadataImporter)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      s_log.InfoFormat ("Importing types...");

      lock (_lockObject)
      {
        using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to import types: {0}."))
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
