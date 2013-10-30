﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.TypeAssembly.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.Caching
{
  /// <summary>
  /// Retrieves the generated type or its constructors for the requested type from the cache or delegates to the contained
  /// <see cref="ITypeAssembler"/> instance.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class TypeCache : ITypeCache
  {
    private readonly string _typeCacheID = Guid.NewGuid().ToString();
    private readonly ConcurrentDictionary<AssembledTypeID, Lazy<Type>> _types = new ConcurrentDictionary<AssembledTypeID, Lazy<Type>>();
    private readonly ConcurrentDictionary<ConstructionKey, Delegate> _constructorCalls = new ConcurrentDictionary<ConstructionKey, Delegate>();

    private readonly ITypeAssembler _typeAssembler;
    private readonly IConstructorDelegateFactory _constructorDelegateFactory;
    private readonly IAssemblyContextPool _assemblyContextPool;

    private readonly Func<AssembledTypeID, Lazy<Type>> _createTypeFunc;
    private readonly Func<ConstructionKey, Delegate> _createConstructorCallFunc;

    public TypeCache (
        ITypeAssembler typeAssembler,
        IConstructorDelegateFactory constructorDelegateFactory,
        IAssemblyContextPool assemblyContextPool)
    {
      ArgumentUtility.CheckNotNull ("typeAssembler", typeAssembler);
      ArgumentUtility.CheckNotNull ("constructorDelegateFactory", constructorDelegateFactory);
      ArgumentUtility.CheckNotNull ("assemblyContextPool", assemblyContextPool);

      _typeAssembler = typeAssembler;
      _constructorDelegateFactory = constructorDelegateFactory;
      _assemblyContextPool = assemblyContextPool;

      _createTypeFunc = CreateType;
      _createConstructorCallFunc = CreateConstructorCall;
    }

    public string ParticipantConfigurationID
    {
      get { return _typeAssembler.ParticipantConfigurationID; }
    }

    public ReadOnlyCollection<IParticipant> Participants
    {
      get { return _typeAssembler.Participants; }
    }

    public Type GetOrCreateType (AssembledTypeID typeID)
    {
      var lazyType = _types.GetOrAdd (typeID, _createTypeFunc);

      try
      {
        return lazyType.Value;
      }
      catch
      {
        // Lazy<T> with ExecutionAndPublication and a create-function caches the exception. 
        // In order to renew the Lazy for another attempt, a replace of the Lazy-object is performed, but only if the _types dictionary
        // still holds the original Lazy (that cached the exception). This avoids a race with a parallel thread that requested the same type.
        if (_types.TryUpdate (typeID, CreateType (typeID), lazyType))
          throw; //TODO RM-5849 Test 

        // Can theoretically cause a StackOverflowException in case of starvation. We are ignoring this very remote possiblity.
        // This code path cannot be tested.
        return GetOrCreateType (typeID);
      }
    }

    private Lazy<Type> CreateType (AssembledTypeID typeID)
    {
      return new Lazy<Type> (
          () =>
          {
            var isAssemblyContextEnqueueRequired = DequeueAssemblyContextOnDemand();
            var assemblyContext = GetDequeuedAssemblyContext();
            try
            {
              return _typeAssembler.AssembleType (typeID, assemblyContext.ParticipantState, assemblyContext.MutableTypeBatchCodeGenerator);
            }
            finally
            {
              EnqueueAssemblyContextOnDemand (isAssemblyContextEnqueueRequired);
            }
          },
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Delegate GetOrCreateConstructorCall (AssembledTypeID typeID, Type delegateType, bool allowNonPublic)
    {
      // Using Assertion.DebugAssert because it will be compiled away.
      Assertion.DebugAssert (delegateType != null && typeof(Delegate).IsAssignableFrom(delegateType));

      var constructionKey = new ConstructionKey (typeID, delegateType, allowNonPublic);
      return _constructorCalls.GetOrAdd (constructionKey, _createConstructorCallFunc);
    }

    private Delegate CreateConstructorCall (ConstructionKey key)
    {
      var assembledType = GetOrCreateType (key.TypeID);
      return _constructorDelegateFactory.CreateConstructorCall (key.TypeID.RequestedType, assembledType, key.DelegateType, key.AllowNonPublic);
    }

    public Type GetOrCreateAdditionalType (object additionalTypeID)
    {
      ArgumentUtility.CheckNotNull ("additionalTypeID", additionalTypeID);

      var isAssemblyContextEnqueueRequired = DequeueAssemblyContextOnDemand();
      var assemblyContext = GetDequeuedAssemblyContext();
      try
      {
        return _typeAssembler.GetOrAssembleAdditionalType (
            additionalTypeID,
            assemblyContext.ParticipantState,
            assemblyContext.MutableTypeBatchCodeGenerator);
      }
      finally
      {
        EnqueueAssemblyContextOnDemand (isAssemblyContextEnqueueRequired);
      }
    }

    public void LoadTypes (IEnumerable<Type> generatedTypes)
    {
      ArgumentUtility.CheckNotNull ("generatedTypes", generatedTypes);

      var assembledTypes = new List<Type>();
      var additionalTypes = new List<Type>();

      foreach (var type in generatedTypes)
      {
        if (_typeAssembler.IsAssembledType (type))
          assembledTypes.Add (type);
        else
          additionalTypes.Add (type);
      }

      // ReSharper disable LoopCanBeConvertedToQuery
      var loadedAssembledTypes = new List<Type>();
      foreach (var assembledType in assembledTypes)
      {
        var typeID = _typeAssembler.ExtractTypeID (assembledType);
        Type assembledTypeForClosure = assembledType;
        if (_types.TryAdd (typeID, new Lazy<Type> (() => assembledTypeForClosure, LazyThreadSafetyMode.None)))
          loadedAssembledTypes.Add (assembledType);
      }
      // ReSharper restore LoopCanBeConvertedToQuery

      //TODO RM-5849: Reenable or completly remove RebuildParticipantState
      //var assemblyContexts = _assemblyContextPool.DequeueAll();
      //try
      //{
      //  _typeAssembler.RebuildParticipantState (loadedAssembledTypes, additionalTypes, assemblyContext.ParticipantState);
      //}
      //finally
      //{
      //  foreach (var assemblyContext in assemblyContexts)
      //    _assemblyContextPool.Enqueue (assemblyContext);
      //}
    }

    private AssemblyContext GetDequeuedAssemblyContext ()
    {
      // CallContext instead thread-static field is used to allow multiple instances of TypeCache to run on the same thread as a nested call-chain.
      var assemblyContext = (AssemblyContext) CallContext.GetData (_typeCacheID);

      Assertion.DebugAssert (assemblyContext != null, "No AssemblyContext was found in CallContext for this TypeCache instance.");

      return assemblyContext;
    }

    private bool DequeueAssemblyContextOnDemand ()
    {
      if (CallContext.GetData (_typeCacheID) == null)
      {
        var assemblyContext = _assemblyContextPool.Dequeue();

        CallContext.SetData (_typeCacheID, assemblyContext);
        return true;
      }
      return false;
    }

    private void EnqueueAssemblyContextOnDemand (bool isAssemblyContextEnqueueRequired)
    {
      if (isAssemblyContextEnqueueRequired)
      {
        var assemblyContext = (AssemblyContext) CallContext.GetData (_typeCacheID);

        _assemblyContextPool.Enqueue (assemblyContext);

        CallContext.FreeNamedDataSlot (_typeCacheID);
      }
    }
  }
}