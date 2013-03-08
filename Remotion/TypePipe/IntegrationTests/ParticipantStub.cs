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
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.IntegrationTests
{
  public class ParticipantStub : IParticipant
  {
    private readonly Action<ProxyType> _typeModification;
    private readonly ICacheKeyProvider _cacheKeyProvider;

    public ParticipantStub (Action<ProxyType> typeModification, ICacheKeyProvider cacheKeyProvider)
    {
      ArgumentUtility.CheckNotNull ("typeModification", typeModification);
      // Cache key provider may be null.

      _typeModification = typeModification;
      _cacheKeyProvider = cacheKeyProvider;
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { return _cacheKeyProvider; }
    }

    public void ModifyType (ProxyType proxyType)
    {
      _typeModification (proxyType);
    }
  }
}