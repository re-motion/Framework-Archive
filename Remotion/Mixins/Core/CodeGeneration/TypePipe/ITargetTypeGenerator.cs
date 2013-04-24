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
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public interface ITargetTypeGenerator
  {
    void AddInterfaces (IEnumerable<Type> interfacesToImplement);
    void AddFields (Type nextCallProxyType);
    void AddTypeInitializations (ClassContext classContext, IEnumerable<Type> mixinTypes);
    void AddInitializations ();

    void ImplementIInitializableMixinTarget (IList<Type> mixinTypes, ConstructorInfo nextCallProxyConstructor);
    void ImplementIMixinTarget (string targetClassName);
    void ImplementIntroducedInterfaces (IEnumerable<InterfaceIntroductionDefinition> introducedInterfaces);
    void ImplementRequiredDuckMethods (TargetClassDefinition targetClassDefinition);
    void ImplementAttributes (TargetClassDefinition targetClassDefinition);

    void AddMixedTypeAttribute (TargetClassDefinition targetClassDefinition);
    void AddDebuggerDisplayAttribute (TargetClassDefinition targetClassDefinition);

    void ImplementOverrides (TargetClassDefinition targetClassDefinition, INextCallProxy nextCallProxy);
    void ImplementOverridingMethods (TargetClassDefinition targetClassDefinition, IList<IMixinInfo> mixinInfos);
  }
}