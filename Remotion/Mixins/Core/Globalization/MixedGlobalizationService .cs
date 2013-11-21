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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Context;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Globalization
{
  public class MixedGlobalizationService : IGlobalizationService
  {
    private readonly ResourceManagerResolver<MultiLingualResourcesAttribute> _resolver =
        new ResourceManagerResolver<MultiLingualResourcesAttribute>();

    private readonly ICache<ClassContext, IResourceManager> _resourceManagerCache =
        CacheFactory.CreateWithLocking<ClassContext, IResourceManager>();

    public IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      var cacheKey = GetCacheKey (typeInformation);
      if (cacheKey == null)
        return NullResourceManager.Instance;

      return _resourceManagerCache.GetOrCreateValue (cacheKey, GetResourceManagerFromType);
    }

    private ClassContext GetCacheKey (ITypeInformation typeInformation)
    {
      var type = typeInformation.AsRuntimeType();
      if (type == null)
        return null;

      //TODO AO: hold ActiveConfiguration in static field - use Interlocked.Compare to check if still current - if not call Cache.Clear
      //refctor cache to use TypeInformation - if no mixins return NullManager
      return MixinConfiguration.ActiveConfiguration.GetContext (type);
    }

    [NotNull]
    private IResourceManager GetResourceManagerFromType (ClassContext classContext)
    {
      var resourceMangers = new List<IResourceManager>();
      CollectResourceManagersRecursively(classContext.Type, resourceMangers);

      if (resourceMangers.Any())
        return new ResourceManagerSet (resourceMangers);

      return NullResourceManager.Instance;
    }

    private void CollectResourceManagersRecursively (Type type, List<IResourceManager> collectedResourceMangers)
    {
      var mixinTypes = MixinTypeUtility.GetMixinTypesExact (type);
      
      foreach (var mixinType in mixinTypes)
        CollectResourceManagersRecursively (mixinType, collectedResourceMangers);

      collectedResourceMangers.AddRange (mixinTypes.Select (mixinType => _resolver.GetResourceManager (mixinType, true)));
    }
  }
}