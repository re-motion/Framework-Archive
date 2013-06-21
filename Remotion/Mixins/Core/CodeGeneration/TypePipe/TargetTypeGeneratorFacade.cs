﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class TargetTypeGeneratorFacade : ITargetTypeModifier
  {
    private readonly INextCallProxyGenerator _nextCallProxyGenerator;

    public TargetTypeGeneratorFacade (INextCallProxyGenerator nextCallProxyGenerator)
    {
      ArgumentUtility.CheckNotNull ("nextCallProxyGenerator", nextCallProxyGenerator);
      _nextCallProxyGenerator = nextCallProxyGenerator;
    }

    public void ModifyTargetType (
        MutableType concreteTarget,
        TargetClassDefinition targetClassDefinition,
        IEnumerable<Type> interfacesToImplement,
        IList<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);
      ArgumentUtility.CheckNotNull ("mixinInfos", mixinInfos);

      var targetTypeGenerator = new TargetTypeGenerator (concreteTarget, new ExpressionBuilder(), new AttributeGenerator(), _nextCallProxyGenerator);
      var mixinTypes = mixinInfos.Select (t => t.MixinType).ToList();

      targetTypeGenerator.AddInterfaces (interfacesToImplement);
      targetTypeGenerator.AddExtensionsField ();
      targetTypeGenerator.AddNextCallProxy (targetClassDefinition, mixinInfos);
      targetTypeGenerator.AddFields();
      targetTypeGenerator.AddTypeInitializations (targetClassDefinition.ConfigurationContext, mixinTypes);
      targetTypeGenerator.AddInitializations();

      targetTypeGenerator.ImplementIInitializableMixinTarget (mixinTypes);
      targetTypeGenerator.ImplementIMixinTarget (targetClassDefinition.Name);
      targetTypeGenerator.ImplementIntroducedInterfaces (targetClassDefinition.ReceivedInterfaces);
      targetTypeGenerator.ImplementRequiredDuckMethods (targetClassDefinition);
      targetTypeGenerator.ImplementAttributes (targetClassDefinition);
      
      targetTypeGenerator.AddMixedTypeAttribute (targetClassDefinition);
      targetTypeGenerator.AddDebuggerDisplayAttribute (targetClassDefinition);
      
      targetTypeGenerator.ImplementOverrides (targetClassDefinition);
      targetTypeGenerator.ImplementOverridingMethods (targetClassDefinition, mixinInfos);
    }
  }
}