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
using Remotion.Mixins.Context;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public interface ITargetTypeModifier
  {
    TargetTypeModifierContext CreateContext (ClassContext classContext, MutableType targetType);

    void ImplementInterfaces (TargetTypeModifierContext context, IEnumerable<Type> interfacesToImplement);
    void AddFields (TargetTypeModifierContext context, Type nextCallProxyType);
    void AddTypeInitializations (TargetTypeModifierContext context, IEnumerable<Type> concreteMixinTypes);
    void AddInitializations (TargetTypeModifierContext context);
    void ImplementIInitializableMixinTarget (TargetTypeModifierContext context, IEnumerable<Type> expectedMixinTypes);
    void ImplementIMixinTarget (TargetTypeModifierContext context);
    void ImplementIntroducedInterfaces (TargetTypeModifierContext context);
    void ImplementRequiredDuckMethods (TargetTypeModifierContext context);
    void AddMixedTypeAttribute (TargetTypeModifierContext context);
    void AddDebuggerAttributes (TargetTypeModifierContext context);
    void ImplementOverrides (TargetTypeModifierContext context);
    void ImplementOverridingMethods (TargetTypeModifierContext context);
  }
}