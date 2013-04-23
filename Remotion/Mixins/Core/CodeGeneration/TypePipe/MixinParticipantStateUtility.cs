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
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class MixinParticipantStateUtility
  {
    public static ConcreteMixinType GetOrGenerateOverrideInterface (
        ITypeAssemblyContext context, MutableType mixinProxyType, ConcreteMixinTypeIdentifier identifier, MixinTypeGenerator mixinTypeGenerator)
    {
      // TODO Review
      // TODO 5370: Locking?

      var concreteMixinTypeCache = GetOrCreateConcreteMixinTypeCache (context.State);

      ConcreteMixinType concreteMixinType;
      if (!concreteMixinTypeCache.TryGetValue (identifier, out concreteMixinType))
      {
        var overrideInterface = mixinTypeGenerator.GenerateOverrides (context);
        var methodWrappers = mixinTypeGenerator.GenerateMethodWrappers();

        concreteMixinType = new ConcreteMixinType (
            identifier, mixinProxyType, overrideInterface.Type, overrideInterface.InterfaceMethodsByOverriddenMethods, methodWrappers);

        context.GenerationCompleted += generatedTypeContext =>
        {
          var completedConcreteMixinType = concreteMixinType.SubstituteMutableReflectionObjects (generatedTypeContext);
          concreteMixinTypeCache.Add (identifier, completedConcreteMixinType);
        };
      }

      return concreteMixinType;
    }

    public static void AddLoadedConcreteMixinType (IDictionary<string, object> participantState, ConcreteMixinType concreteMixinType)
    {
      var concreteMixinTypeCache = GetOrCreateConcreteMixinTypeCache (participantState);

      // TODO Review
      // what if identifier already present?
      if (!concreteMixinTypeCache.ContainsKey (concreteMixinType.Identifier))
      {
        concreteMixinTypeCache.Add (concreteMixinType.Identifier, concreteMixinType);
      }
    }

    private static IDictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType> GetOrCreateConcreteMixinTypeCache (
        IDictionary<string, object> participantState)
    {
      const string key = "ConcreteMixinTypes";
      var concreteMixinTypeCache = (Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>) participantState.GetValueOrDefault (key);
      if (concreteMixinTypeCache == null)
      {
        concreteMixinTypeCache = new Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>();
        participantState.Add (key, concreteMixinTypeCache);
      }

      return concreteMixinTypeCache;
    }
  }
}