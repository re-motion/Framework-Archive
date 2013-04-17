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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class TargetTypeModifierFacade : ITargetTypeModifierFacade
  {
    private readonly ITargetTypeModifier _targetTypeModifier;

    public TargetTypeModifierFacade (ITargetTypeModifier targetTypeModifier)
    {
      ArgumentUtility.CheckNotNull ("targetTypeModifier", targetTypeModifier);

      _targetTypeModifier = targetTypeModifier;
    }

    public void ModifyTargetType (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _targetTypeModifier.AddInterfaces (context, context.InterfacesToImplement);
      _targetTypeModifier.AddFields (context, context.NextCallProxyGenerator.Type);
      _targetTypeModifier.AddTypeInitializations (context, context.TargetClassDefinition.ConfigurationContext, context.MixinTypes);
      _targetTypeModifier.AddInitializations (context);

      _targetTypeModifier.ImplementIInitializableMixinTarget (context, context.MixinTypes);
      _targetTypeModifier.ImplementIMixinTarget (context);
      _targetTypeModifier.ImplementIntroducedInterfaces (context, context.TargetClassDefinition.ReceivedInterfaces);
      _targetTypeModifier.ImplementRequiredDuckMethods (context);

      _targetTypeModifier.AddMixedTypeAttribute (context, context.TargetClassDefinition);
      _targetTypeModifier.AddDebuggerDisplayAttribute (context, context.TargetClassDefinition);

      _targetTypeModifier.ImplementOverrides (context);
      _targetTypeModifier.ImplementOverridingMethods (context, context.ConcreteMixinTypes);
    }
  }
}