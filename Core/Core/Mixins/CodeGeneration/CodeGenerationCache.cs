/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using System.Reflection;
using System.Linq;

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
    private readonly Cache<TargetClassDefinition, Type> _typeCache = new Cache<TargetClassDefinition, Type> ();
    private readonly Cache<object, ConcreteMixinType> _mixinTypeCache = new Cache<object, ConcreteMixinType> ();

    public CodeGenerationCache (ConcreteTypeBuilder concreteTypeBuilder)
    {
      ArgumentUtility.CheckNotNull ("concreteTypeBuilder", concreteTypeBuilder);
      _concreteTypeBuilder = concreteTypeBuilder;
    }

    public Type GetConcreteType (IModuleManager moduleManager, TargetClassDefinition targetClassDefinition, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _typeCache.GetOrCreateValue (
            targetClassDefinition,
            key => GenerateConcreteType (moduleManager, key, nameProvider, mixinNameProvider));
      }
    }

    private Type GenerateConcreteType (IModuleManager moduleManager, TargetClassDefinition targetClassDefinition, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      s_log.InfoFormat ("Generating type for {0}.", targetClassDefinition.ConfigurationContext);
      using (new CodeGenerationTimer ())
      {
        ITypeGenerator generator = moduleManager.CreateTypeGenerator (
            this,
            targetClassDefinition,
            nameProvider,
            mixinNameProvider);

        return generator.GetBuiltType();
      }
    }

    public ConcreteMixinType GetConcreteMixinType (ITypeGenerator mixedTypeGenerator, MixinDefinition mixinDefinition, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("mixedTypeGenerator", mixedTypeGenerator);
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _mixinTypeCache.GetOrCreateValue (
              mixinDefinition.GetConcreteMixinTypeCacheKey(),
              key => GenerateConcreteMixinType (mixedTypeGenerator, mixinDefinition, mixinNameProvider));
      }
    }

    private ConcreteMixinType GenerateConcreteMixinType(ITypeGenerator mixedTypeGenerator, MixinDefinition mixinDefinition, INameProvider mixinNameProvider)
    {
      return _concreteTypeBuilder.Scope.CreateMixinTypeGenerator (mixedTypeGenerator, mixinDefinition, mixinNameProvider).GetBuiltType ();
    }

    public ConcreteMixinType GetConcreteMixinTypeFromCacheOnly (MixinDefinition mixinDefinition)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      lock (_lockObject)
      {
        ConcreteMixinType type;
        _mixinTypeCache.TryGetValue (mixinDefinition, out type);
        return type;
      }
    }

    public void ImportTypes (IEnumerable<Type> types, IConcreteTypeMetadataImporter metadataImporter)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      lock (_lockObject)
      {
        foreach (Type type in types)
        {
          ImportConcreteMixedType(metadataImporter, type);
          ImportConcreteMixinType(metadataImporter, type);
        }
      }
    }

    private void ImportConcreteMixedType(IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      foreach (TargetClassDefinition mixedTypeMetadata in metadataImporter.GetMetadataForMixedType (type, TargetClassDefinitionCache.Current))
        _typeCache.GetOrCreateValue (mixedTypeMetadata, delegate { return type; });
    }


    private void ImportConcreteMixinType (IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      var mixinDefinitions = metadataImporter.GetMetadataForMixinType (type, TargetClassDefinitionCache.Current).ToArray ();
      if (mixinDefinitions.Length > 0)
      {
        var methodWrappers = metadataImporter.GetMethodWrappersForMixinType (type);
        foreach (MixinDefinition mixinDefinition in mixinDefinitions)
        {
          var concreteMixinType = new ConcreteMixinType (mixinDefinition, type);
          foreach (Tuple<MethodInfo, MethodInfo> wrapper in methodWrappers)
            concreteMixinType.AddMethodWrapper (wrapper.A, wrapper.B);

          _mixinTypeCache.GetOrCreateValue (mixinDefinition, delegate { return concreteMixinType; });
        }
      }
    }
  }
}