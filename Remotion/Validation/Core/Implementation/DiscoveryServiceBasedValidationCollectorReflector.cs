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
  public class DiscoveryServiceBasedValidationCollectorReflector : IValidationCollectorReflector
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly Lazy<MultiDictionary<Type, Type>> _typeCollectors;
    
    public DiscoveryServiceBasedValidationCollectorReflector ()
        : this (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService())
    {
    }

    public DiscoveryServiceBasedValidationCollectorReflector (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
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
      var allCollectors = _typeDiscoveryService.GetTypes (typeof (IComponentValidationCollector), true).Cast<Type> ();
      var typeCollectors = new MultiDictionary<Type, Type> ();
      foreach (var collectorType in allCollectors)
      {
        if (collectorType.IsAbstract || collectorType.IsInterface || collectorType.IsGenericTypeDefinition || collectorType.IsDefined (typeof (ApplyProgrammaticallyAttribute), false))
          continue;

        // var type = IValidatedTypeResolver.GetValidatedType (collectorType) //stategy with decoration, if returns null, throw invalidoperationexception for unhandled collector 
        // typeCollectors[type].add (collectorType)

        var genericType = collectorType.GetFirstGenericTypeParameterInHierarchy (); // only in generic implementation
        if (collectorType.IsDefined (typeof (ApplyWithClassAttribute), false))  //TODO AO: move to new class (composite)
        {
          var classType = AttributeUtility.GetCustomAttribute<ApplyWithClassAttribute> (collectorType, false).ClassType;
          CheckGenericTypeAssignableFromDefinedType (collectorType, genericType, classType, typeof (ApplyWithClassAttribute).Name);

          typeCollectors[classType].Add (collectorType);
        }
        else if (collectorType.IsDefined (typeof (ApplyWithMixinAttribute), false)) //TODO AO: move to mixin assembly
        {
          var mixinType = AttributeUtility.GetCustomAttribute<ApplyWithMixinAttribute> (collectorType, false).MixinType;
          typeCollectors[mixinType].Add (collectorType);
        }
        else
        {
          typeCollectors[genericType].Add (collectorType);
        }
      }

      return typeCollectors;
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