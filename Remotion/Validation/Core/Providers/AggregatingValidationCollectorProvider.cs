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
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Providers
{
  //TODO MK: Review
  public class AggregatingValidationCollectorProvider : IValidationCollectorProvider
  {
    private readonly IInvolvedTypeProvider _involvedTypeProvider;
    private readonly IValidationCollectorProvider[] _validationCollectorProviders;

    public AggregatingValidationCollectorProvider (
        IInvolvedTypeProvider involvedTypeProvider,
        IValidationCollectorProvider[] validationCollectorProviders)
    {
      ArgumentUtility.CheckNotNull ("involvedTypeProvider", involvedTypeProvider);
      ArgumentUtility.CheckNotNull ("validationCollectorProviders", validationCollectorProviders);

      _involvedTypeProvider = involvedTypeProvider;
      _validationCollectorProviders = validationCollectorProviders;
    }

    public IInvolvedTypeProvider InvolvedTypeProvider
    {
      get { return _involvedTypeProvider; }
    }

    public IEnumerable<IValidationCollectorProvider> ValidationCollectorProviders
    {
      get { return _validationCollectorProviders.AsReadOnly(); }
    }

    public IEnumerable<IEnumerable<ValidationCollectorInfo>> GetValidationCollectors (Type[] types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var typeGroups = types.SelectMany (t => _involvedTypeProvider.GetTypes (t)).Select(g=>g.ToArray());

      return typeGroups
          .Aggregate (
              Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>(),
              (current, @group) => current.Concat (_validationCollectorProviders.SelectMany (p => p.GetValidationCollectors (@group))))
          .ToArray();
    }
  }
}