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
  // TODO 5370.
  public class MixinParticipant : IParticipant
  {
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IMixinTypeProvider _mixinTypeProvider;
    private readonly INextCallProxyGenerator _nextCallProxyGenerator;
    private readonly ITargetTypeModifier _targetTypeModifier;
    private readonly IConcreteTypeMetadataImporter _concreteTypeMetadataImporter;

    public MixinParticipant ()
        : this (
            new ConfigurationProvider(),
            new MixinTypeGeneratorFacade(),
            new NextCallProxyGenerator(),
            new TargetTypeGeneratorFacade(),
            new AttributeBasedMetadataImporter())
    {
    }

    private MixinParticipant (
        IConfigurationProvider configurationProvider,
        IMixinTypeProvider mixinTypeProvider,
        INextCallProxyGenerator nextCallProxyGenerator,
        ITargetTypeModifier targetTypeModifier,
        IConcreteTypeMetadataImporter concreteTypeMetadataImporter)
    {
      ArgumentUtility.CheckNotNull ("configurationProvider", configurationProvider);
      ArgumentUtility.CheckNotNull ("mixinTypeProvider", mixinTypeProvider);
      ArgumentUtility.CheckNotNull ("nextCallProxyGenerator", nextCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("targetTypeModifier", targetTypeModifier);
      ArgumentUtility.CheckNotNull ("concreteTypeMetadataImporter", concreteTypeMetadataImporter);

      _configurationProvider = configurationProvider;
      _mixinTypeProvider = mixinTypeProvider;
      _nextCallProxyGenerator = nextCallProxyGenerator;
      _targetTypeModifier = targetTypeModifier;
      _concreteTypeMetadataImporter = concreteTypeMetadataImporter;
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { return new MixinParticipantCacheKeyProvider (_concreteTypeMetadataImporter); }
    }

    public void Participate (ITypeAssemblyContext typeAssemblyContext)
    {
      ArgumentUtility.CheckNotNull ("typeAssemblyContext", typeAssemblyContext);

      var target = typeAssemblyContext.RequestedType;
      var concreteTarget = typeAssemblyContext.ProxyType;

      var targetClassDefinition = _configurationProvider.GetTargetClassDefinition (target);
      if (targetClassDefinition == null)
        return;

      var mixinInfos = targetClassDefinition.Mixins.Select (m => _mixinTypeProvider.GetMixinInfo (typeAssemblyContext, m)).ToList();
      var interfacesToImplement = _configurationProvider.GetInterfacesToImplement (targetClassDefinition, mixinInfos);
      var targetTypeForNextCall = _nextCallProxyGenerator.GetTargetTypeWrapper (concreteTarget);
      var nextCallProxy = _nextCallProxyGenerator.Create (typeAssemblyContext, targetClassDefinition, mixinInfos, targetTypeForNextCall);

      _targetTypeModifier.ModifyTargetType (concreteTarget, targetClassDefinition, nextCallProxy, interfacesToImplement, mixinInfos);
    }

    public void RebuildState (LoadedTypesContext loadedTypesContext)
    {
      ArgumentUtility.CheckNotNull ("loadedTypesContext", loadedTypesContext);

      foreach (var additionalType in loadedTypesContext.AdditionalTypes)
      {
        var conreteMixinType = _concreteTypeMetadataImporter.GetMetadataForMixinType (additionalType);
        if (conreteMixinType != null)
          MixinTypeGeneratorFacade.AddLoadedConcreteMixinType (loadedTypesContext.State, conreteMixinType);
      }
    }

    public void HandleNonSubclassableType (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      var targetClassDefinition = _configurationProvider.GetTargetClassDefinition (requestedType);
      Assertion.IsNull (
          targetClassDefinition,
          "There should be no mixin configuration for a non-subclassable type; "
          + "and if there was (i.e., user error) an configuration validation exception should have been thrown.");
    }
  }
}