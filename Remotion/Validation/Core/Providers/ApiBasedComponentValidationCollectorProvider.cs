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
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Use this class to retrieve the <see cref="IComponentValidationCollector"/>s for a <see cref="Type"/> based on reflection metadata.
  /// </summary>
  public class ApiBasedComponentValidationCollectorProvider : IValidationCollectorProvider
  {
    private readonly IValidationCollectorReflector _validationCollectorReflector;

    public ApiBasedComponentValidationCollectorProvider (IValidationCollectorReflector validationCollectorReflector)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorReflector", validationCollectorReflector);

      _validationCollectorReflector = validationCollectorReflector;
    }

    public IValidationCollectorReflector ValidationCollectorReflector
    {
      get { return _validationCollectorReflector; }
    }

    public IEnumerable<IEnumerable<ValidationCollectorInfo>> GetValidationCollectors (IEnumerable<Type> types)
        //TODO AO: should be decorated by AN to return collector-groups sorted by component! -> integration test!
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var result = types
          .SelectMany (_validationCollectorReflector.GetCollectorsForType)
          .Select (c => new ValidationCollectorInfo ((IComponentValidationCollector) Activator.CreateInstance (c), GetType())).ToArray();
      return result.Any() ? new[] { result } : Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>();
    }
  }
}