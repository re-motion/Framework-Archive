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
using System.Linq;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public class MixinParticipant : IParticipant
  {
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IMixinTypeGeneratorFacade _mixinTypeGeneratorFacade;
    private readonly INextCallProxyGenerator _nextCallProxyGenerator;
    private readonly ITargetTypeModifierFacade _targetTypeModifierFacade;

    public MixinParticipant (
        IConfigurationProvider configurationProvider,
        IMixinTypeGeneratorFacade mixinTypeGeneratorFacade,
        INextCallProxyGenerator nextCallProxyGenerator,
        ITargetTypeModifierFacade targetTypeModifierFacade)
    {
      ArgumentUtility.CheckNotNull ("configurationProvider", configurationProvider);
      ArgumentUtility.CheckNotNull ("mixinTypeGeneratorFacade", mixinTypeGeneratorFacade);
      ArgumentUtility.CheckNotNull ("nextCallProxyGenerator", nextCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("targetTypeModifierFacade", targetTypeModifierFacade);

      _configurationProvider = configurationProvider;
      _mixinTypeGeneratorFacade = mixinTypeGeneratorFacade;
      _nextCallProxyGenerator = nextCallProxyGenerator;
      _targetTypeModifierFacade = targetTypeModifierFacade;
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { throw new System.NotImplementedException(); }
    }

    public void Participate (ITypeAssemblyContext typeAssemblyContext)
    {
      ArgumentUtility.CheckNotNull ("typeAssemblyContext", typeAssemblyContext);

      var target = typeAssemblyContext.RequestedType;
      var concreteTarget = typeAssemblyContext.ProxyType;

      var targetClassDefinition = _configurationProvider.GetTargetClassDefinition (target);
      var concreteMixinTypesWithNulls = _mixinTypeGeneratorFacade.GenerateConcreteMixinTypesWithNulls (typeAssemblyContext, targetClassDefinition.Mixins).ToList();
      var interfacesToImplement = _configurationProvider.GetInterfacesToImplement (targetClassDefinition, concreteMixinTypesWithNulls);
      var targetTypeForNextCall = _nextCallProxyGenerator.GetTargetTypeWrapper (concreteTarget);
      var nextCallProxy = _nextCallProxyGenerator.Create (
          typeAssemblyContext, targetClassDefinition, concreteMixinTypesWithNulls, targetTypeForNextCall);

      _targetTypeModifierFacade.ModifyTargetType (
          concreteTarget, targetClassDefinition, nextCallProxy, interfacesToImplement, concreteMixinTypesWithNulls);
    }

    public void RebuildState (LoadedTypesContext loadedTypesContext)
    {
      throw new System.NotImplementedException();
    }
  }
}