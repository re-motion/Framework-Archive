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
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using Remotion.Collections;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;
using Remotion.Validation.Attributes;

namespace Remotion.Validation.Implementation
{
  public class DiscoveryServiceBasedValidationCollectorReflector : IValidationCollectorReflector
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly Lazy<MultiDictionary<Type, Type>> _typeCollectors;
    private readonly IValidatedTypeResolver _validatedTypeResolver;

    public DiscoveryServiceBasedValidationCollectorReflector (IValidatedTypeResolver validatedTypeResolver)
        : this (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService(), validatedTypeResolver)
    {
    }

    public DiscoveryServiceBasedValidationCollectorReflector (
        ITypeDiscoveryService typeDiscoveryService,
        IValidatedTypeResolver validatedTypeResolver)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);
      ArgumentUtility.CheckNotNull ("validatedTypeResolver", validatedTypeResolver);

      _typeDiscoveryService = typeDiscoveryService;
      _validatedTypeResolver = validatedTypeResolver;
      _typeCollectors = new Lazy<MultiDictionary<Type, Type>> (Initialize, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public IEnumerable<Type> GetCollectorsForType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _typeCollectors.Value[type];
    }

    private MultiDictionary<Type, Type> Initialize ()
    {
      //TOOD AO: add integration test for IComponentValidationCollector
      var allCollectors = _typeDiscoveryService.GetTypes (typeof (IComponentValidationCollector), true).Cast<Type>();
      var typeCollectors = new MultiDictionary<Type, Type>();
      foreach (var collectorType in allCollectors)
      {
        if (collectorType.IsAbstract || collectorType.IsInterface || collectorType.IsGenericTypeDefinition
            || collectorType.IsDefined (typeof (ApplyProgrammaticallyAttribute), false))
          continue;

        var type = _validatedTypeResolver.GetValidatedType (collectorType);
        if (type == null)
          throw new InvalidOperationException (string.Format ("No validated type could be resolved for collector '{0}'.", collectorType.Name));
        typeCollectors[type].Add (collectorType);
      }

      return typeCollectors;
    }
  }
}