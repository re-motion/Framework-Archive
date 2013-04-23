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
using Remotion.TypePipe.MutableReflection;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class TargetTypeGeneratorFacade : ITargetTypeModifier
  {
    public void ModifyTargetType (
        MutableType concreteTarget,
        TargetClassDefinition targetClassDefinition,
        INextCallProxy nextCallProxy,
        IEnumerable<Type> interfacesToImplement,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls)
    {

      var modifier = new TargetTypeGenerator (concreteTarget, new ExpressionBuilder(), new AttributeGenerator());
      var expectedMixinTypes = GetExpectedMixinTypes (targetClassDefinition, concreteMixinTypesWithNulls);

      modifier.AddInterfaces (interfacesToImplement);
      modifier.AddFields (nextCallProxy.Type);
      modifier.AddTypeInitializations (targetClassDefinition.ConfigurationContext, expectedMixinTypes);
      modifier.AddInitializations();
      
      modifier.ImplementIInitializableMixinTarget (expectedMixinTypes, nextCallProxy.Constructor);
      modifier.ImplementIMixinTarget (targetClassDefinition.Name);
      modifier.ImplementIntroducedInterfaces (targetClassDefinition.ReceivedInterfaces);
      modifier.ImplementRequiredDuckMethods (targetClassDefinition);
      modifier.ImplementAttributes (targetClassDefinition);
      
      modifier.AddMixedTypeAttribute (targetClassDefinition);
      modifier.AddDebuggerDisplayAttribute (targetClassDefinition);
      
      modifier.ImplementOverrides (targetClassDefinition, nextCallProxy);
      modifier.ImplementOverridingMethods (targetClassDefinition, concreteMixinTypesWithNulls);
    }

    private static List<Type> GetExpectedMixinTypes (
        TargetClassDefinition targetClassDefinition, IList<ConcreteMixinType> concreteMixinTypesWithNulls)
    {
      // Get derived mixin type or the original mixin type if there is no derived.
      return concreteMixinTypesWithNulls.Select ((m, i) => m != null ? m.GeneratedType : targetClassDefinition.Mixins[i].Type).ToList();
    }
  }
}