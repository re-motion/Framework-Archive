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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  public sealed class GlobalizationService : IGlobalizationService
  {
    private readonly ResourceManagerResolver<MultiLingualResourcesAttribute> _resolver =
        new ResourceManagerResolver<MultiLingualResourcesAttribute> ();

    private readonly ICache<ITypeInformation, IResourceManager> _resourceManagerCache =
        CacheFactory.CreateWithLocking<ITypeInformation, IResourceManager> ();

    //TODO AO: cache as static in extension class - later replace instantiation with IoC
    private readonly TypeConversionProvider _typeConversionProvider;

    public GlobalizationService ()
    {
      _typeConversionProvider = TypeConversionProvider.Create();
    }

    public IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      return _resourceManagerCache.GetOrCreateValue (typeInformation, GetResourceManagerImplementation);
    }

    [NotNull]
    private IResourceManager GetResourceManagerImplementation (ITypeInformation typeInformation)
    {
      if (!_typeConversionProvider.CanConvert (typeInformation.GetType (), typeof (Type)))
        return NullResourceManager.Instance;

      var type = (Type) _typeConversionProvider.Convert (typeInformation.GetType (), typeof (Type), typeInformation);

      return _resolver.GetResourceManager (type, true);
    }
  }
}