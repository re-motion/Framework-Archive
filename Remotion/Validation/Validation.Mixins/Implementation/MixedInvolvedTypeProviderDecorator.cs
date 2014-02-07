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
using Remotion.Mixins;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Mixins.Implementation
{
  //TODO AO: change to composite -> check with MK: GetConcreteMixedType requied => decorator ok??
  public class MixedInvolvedTypeProviderDecorator : IInvolvedTypeProvider
  {
    private readonly IInvolvedTypeProvider _involvedTypeProvider;

    public MixedInvolvedTypeProviderDecorator (IInvolvedTypeProvider involvedTypeProvider)
    {
      ArgumentUtility.CheckNotNull ("involvedTypeProvider", involvedTypeProvider);

      _involvedTypeProvider = involvedTypeProvider;
    }

    public IValidationTypeFilter ValidationTypeFilter
    {
      get { return _involvedTypeProvider.ValidationTypeFilter; }
    }

    public IEnumerable<IEnumerable<Type>> GetTypes (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var concreteOrMixedType = MixinTypeUtility.GetConcreteMixedType (type);

      var involvedTypes = _involvedTypeProvider.GetTypes (concreteOrMixedType);
      var involvedMixins = GetMixins (type);
      return involvedTypes.Concat (involvedMixins);
    }

    private IEnumerable<IEnumerable<Type>> GetMixins (Type type)
    {
      return MixinTypeUtility.GetMixinTypesExact (type).Where (ValidationTypeFilter.IsValid).Select (mixinType => new[] { mixinType }).ToArray();
    }
  }
}