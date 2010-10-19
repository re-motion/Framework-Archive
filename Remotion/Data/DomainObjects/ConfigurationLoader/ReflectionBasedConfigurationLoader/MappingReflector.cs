// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel.Design;
using System.Linq;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO COMMONS-825: join again with base class
  public class MappingReflector : MappingReflectorBase
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;

    //TODO: Test
    public MappingReflector ()
    {
      _typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService();
    }

    public MappingReflector (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
    }

    protected override IEnumerable<Type> GetDomainObjectTypes ()
    {
      return (from type in _typeDiscoveryService.GetTypes (typeof (DomainObject), false).Cast<Type>()
              where !type.IsDefined (typeof (IgnoreForMappingConfigurationAttribute), false)
                //TODO COMMONS-825: test this
              && !ReflectionUtility.IsDomainObjectBase (type)
              select type).Distinct();
    }
  }
}
