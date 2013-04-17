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
using System.Linq;
using Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class ConfigurationProvider : IConfigurationProvider
  {
    public TargetClassDefinition GetTargetClassDefinition (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (requestedType);
      return TargetClassDefinitionFactory.CreateAndValidate (classContext);
    }

    public IEnumerable<Type> GetInterfacesToImplement (
        TargetClassDefinition targetClassDefinition, IEnumerable<ConcreteMixinType> concreteMixinTypesWithNulls)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("concreteMixinTypesWithNulls", concreteMixinTypesWithNulls);

      var implementedInterfaceFinder = new ImplementedInterfaceFinder (
          targetClassDefinition.ImplementedInterfaces,
          targetClassDefinition.ReceivedInterfaces,
          targetClassDefinition.RequiredTargetCallTypes,
          concreteMixinTypesWithNulls.Where (t => t != null));

      return implementedInterfaceFinder.GetInterfacesToImplement();
    }
  }
}