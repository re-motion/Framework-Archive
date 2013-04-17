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
    private readonly IMixinParticipantHelper _helper;
    private readonly ITargetTypeModifier _targetTypeModifier;

    public MixinParticipant (
        IConfigurationProvider configurationProvider, IMixinParticipantHelper helper, ITargetTypeModifier targetTypeModifier)
    {
      ArgumentUtility.CheckNotNull ("configurationProvider", configurationProvider);
      ArgumentUtility.CheckNotNull ("helper", helper);
      ArgumentUtility.CheckNotNull ("targetTypeModifier", targetTypeModifier);

      _configurationProvider = configurationProvider;
      _helper = helper;
      _targetTypeModifier = targetTypeModifier;
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { throw new System.NotImplementedException(); }
    }

    public void Participate (ITypeAssemblyContext typeAssemblyContext)
    {
      ArgumentUtility.CheckNotNull ("typeAssemblyContext", typeAssemblyContext);

      var targetClassDefinition = _configurationProvider.GetTargetClassDefinition (typeAssemblyContext.RequestedType);
      var mixinInfos = _helper.GetConcreteMixinTypes (targetClassDefinition.Mixins);
      var interfacesToImplement = _configurationProvider.GetInterfacesToImplement (targetClassDefinition, mixinInfos);

      // TODO: NextCallProxy?!?
      

      // TODO: Add TargetClassDefiniton to context

      //var context = _targetTypeModifier.CreateContext (typeAssemblyContext.ProxyType);
      //_targetTypeModifier.ImplementInterfaces (context, interfacesToImplement);
      //_targetTypeModifier.AddFields (context, null); // TODO
      //_targetTypeModifier.AddTypeInitializations (context, null, null); // TODO
      //_targetTypeModifier.AddInitializations();
      //_targetTypeModifier.ImplementIInitializableMixinTarget (context);
      //_targetTypeModifier.ImplementIMixinTarget (context);
      //_targetTypeModifier.ImplementIntroducedInterfaces (context);
      //_targetTypeModifier.ImplementRequiredDuckMethods (context);
      //_targetTypeModifier.AddMixedTypeAttribute (context);
      //_targetTypeModifier.AddDebuggerDisplayAttribute (context);
      //_targetTypeModifier.ImplementOverrides (context);
      //_targetTypeModifier.ImplementOverridingMethods (context);
    }

    public void RebuildState (LoadedTypesContext loadedTypesContext)
    {
      throw new System.NotImplementedException();
    }
  }
}