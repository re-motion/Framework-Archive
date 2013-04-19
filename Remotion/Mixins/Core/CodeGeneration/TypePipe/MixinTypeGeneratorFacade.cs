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
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;
using System.Linq;
using Remotion.Collections;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class MixinTypeGeneratorFacade : IMixinTypeGeneratorFacade
  {
    public IEnumerable<ConcreteMixinType> GenerateConcreteMixinTypesWithNulls (ITypeAssemblyContext context, IEnumerable<MixinDefinition> mixins)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("mixins", mixins);

      return mixins.Select (m => GenerateConcreteMixinTypeOrNull (context, m));
    }

    private ConcreteMixinType GenerateConcreteMixinTypeOrNull (ITypeAssemblyContext context, MixinDefinition mixin)
    {
      if (!mixin.NeedsDerivedMixinType())
        return null;

      return GenerateConcreteMixinType (context, mixin.GetConcreteMixinTypeIdentifier());
    }

    private ConcreteMixinType GenerateConcreteMixinType (ITypeAssemblyContext context, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      var mixinProxyType = context.CreateProxy (concreteMixinTypeIdentifier.MixinType);

      var generator = new MixinTypeGenerator (concreteMixinTypeIdentifier, mixinProxyType, new AttributeGenerator(), new ExpressionBuilder());
      generator.AddInterfaces();
      generator.AddFields();
      generator.AddTypeInitializer();
      generator.ImplementGetObjectData();

      generator.AddMixinTypeAttribute();
      generator.AddDebuggerAttributes();

      var overrideInterface = GetOrGenerateOverrideInterface (context, concreteMixinTypeIdentifier, generator);
      var methodWrappers = generator.GenerateMethodWrappers();

      return new ConcreteMixinType (
          concreteMixinTypeIdentifier, mixinProxyType, overrideInterface.Type, overrideInterface.InterfaceMethodsByOverriddenMethods, methodWrappers);
    }

    private static OverrideInterface GetOrGenerateOverrideInterface (
        ITypeAssemblyContext context, ConcreteMixinTypeIdentifier key, MixinTypeGenerator mixinTypeGenerator)
    {
      // TODO 5370: Locking?

      var concreteMixinTypeCache = GetOrCreateCacheConcreteMixinTypeCache (context.State);

      OverrideInterface overrideInterface;
      if (!concreteMixinTypeCache.TryGetValue (key, out overrideInterface))
      {
        overrideInterface = mixinTypeGenerator.GenerateOverrides (context);

        context.GenerationCompleted += generatedTypeContext =>
        {
          var substitutedOverrideInterface = overrideInterface.SubstituteForRealReflectionObjects (generatedTypeContext);
          concreteMixinTypeCache.Add (key, substitutedOverrideInterface);
        };
      }

      return overrideInterface;
    }

    private static Dictionary<ConcreteMixinTypeIdentifier, OverrideInterface> GetOrCreateCacheConcreteMixinTypeCache (
        IDictionary<string, object> participantState)
    {
      const string key = "ConcreteMixinTypes";
      var concreteMixinTypeCache = (Dictionary<ConcreteMixinTypeIdentifier, OverrideInterface>) participantState.GetValueOrDefault (key);
      if (concreteMixinTypeCache == null)
      {
        concreteMixinTypeCache = new Dictionary<ConcreteMixinTypeIdentifier, OverrideInterface>();
        participantState.Add (key, concreteMixinTypeCache);
      }

      return concreteMixinTypeCache;
    }
  }
}