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
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Implementation
{
  public class DiscoveryServiceBasedTypeCollectorReflector : ITypeCollectorReflector
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly MultiDictionary<Type, Type> _typeCollectors;

    public DiscoveryServiceBasedTypeCollectorReflector ()
        : this (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService())
    {
    }

    public DiscoveryServiceBasedTypeCollectorReflector (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
      _typeCollectors = new MultiDictionary<Type, Type> ();

      //TODO AO: Move to lazy initiazied _typeCollectors 
      //_typeCollectors = new Lazy<MultiDictionary<Type, Type>> (Initialize, LazyThreadSafetyMode.ExecutionAndPublication);
      Initialize ();
    }

    public IEnumerable<Type> GetCollectorsForType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _typeCollectors[type];
    }

    private void Initialize ()
    {
      var allCollectors = _typeDiscoveryService.GetTypes (typeof (IComponentValidationCollector), true).Cast<Type> ();
      foreach (var collectorType in allCollectors)
      {
        if (collectorType.IsAbstract || collectorType.IsInterface || collectorType.IsGenericTypeDefinition || collectorType.IsDefined (typeof (ApplyProgrammaticallyAttribute), false))
          continue;

        var genericType = collectorType.GetFirstGenericTypeParameterInHierarchy ();
        if (collectorType.IsDefined (typeof (ApplyWithClassAttribute), false))
        {
          var classType = AttributeUtility.GetCustomAttribute<ApplyWithClassAttribute> (collectorType, false).ClassType;
          CheckGenericTypeAssignableFromDefinedType (collectorType, genericType, classType, typeof (ApplyWithClassAttribute).Name);

          _typeCollectors[classType].Add (collectorType);
        }
        else if (collectorType.IsDefined (typeof (ApplyWithMixinAttribute), false))
        {
          var mixinType = AttributeUtility.GetCustomAttribute<ApplyWithMixinAttribute> (collectorType, false).MixinType;
          _typeCollectors[mixinType].Add (collectorType);
        }
        else
        {
          _typeCollectors[genericType].Add (collectorType);
        }
      }
    }

    private static void CheckGenericTypeAssignableFromDefinedType (Type collectorType, Type genericType, Type classOrMixinTypee, string attributeName)
    {
      if (!genericType.IsAssignableFrom (classOrMixinTypee))
      {
        throw new InvalidOperationException (
            string.Format (
                "Invalid '{0}'-definition for collector '{1}': type '{2}' is not assignable from '{3}'.",
                attributeName,
                collectorType.FullName,
                genericType.FullName,
                classOrMixinTypee.FullName));
      }
    }


  }
}