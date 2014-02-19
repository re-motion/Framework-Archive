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
using System.Diagnostics;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Returns all base classes, interfaces and mixins for the given type hierarchy.
  /// </summary>
  public class InvolvedTypeProvider : IInvolvedTypeProvider
  {
    private readonly Func<IEnumerable<Type>, IEnumerable<Type>> _hierarchyLevelsubSort;
    private readonly IValidationTypeFilter _validationTypeFilter;

    public static IInvolvedTypeProvider Create (Func<IEnumerable<Type>, IEnumerable<Type>> hierarchyLevelsubSort)
    {
      ArgumentUtility.CheckNotNull ("hierarchyLevelsubSort", hierarchyLevelsubSort);

      return Create (hierarchyLevelsubSort, new CompoundValidationTypeFilter (Enumerable.Empty<IValidationTypeFilter>()));
    }

    public static IInvolvedTypeProvider Create (
        Func<IEnumerable<Type>, IEnumerable<Type>> hierarchyLevelsubSort,
        IValidationTypeFilter validationTypeFilter)
    {
      ArgumentUtility.CheckNotNull ("hierarchyLevelsubSort", hierarchyLevelsubSort);
      ArgumentUtility.CheckNotNull ("validationTypeFilter", validationTypeFilter);

      return new InvolvedTypeProvider (hierarchyLevelsubSort, validationTypeFilter);
    }

    public InvolvedTypeProvider ()
        : this (c => c.OrderBy (t => t.Name), new CompoundValidationTypeFilter (Enumerable.Empty<IValidationTypeFilter>()))
    {
    }

    protected InvolvedTypeProvider (Func<IEnumerable<Type>, IEnumerable<Type>> hierarchyLevelsubSort, IValidationTypeFilter validationTypeFilter)
    {
      ArgumentUtility.CheckNotNull ("hierarchyLevelsubSort", hierarchyLevelsubSort);
      ArgumentUtility.CheckNotNull ("validationTypeFilter", validationTypeFilter);

      _hierarchyLevelsubSort = hierarchyLevelsubSort;
      _validationTypeFilter = validationTypeFilter;
    }

    public IEnumerable<IEnumerable<Type>> GetTypes (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var inheritanceHierarchy = GetInheritanceHierarchy (type).ToArray();
      foreach (var classType in inheritanceHierarchy)
      {
        foreach (var interfaceGroup in GetInterfaces (classType))
          yield return interfaceGroup;

        yield return new[] { classType };
      }
    }

    private IEnumerable<Type> GetInheritanceHierarchy (Type type)
    {
      return type.CreateSequence (t => t.BaseType).Where (_validationTypeFilter.IsValidatableType).Reverse();
    }

    private IEnumerable<IEnumerable<Type>> GetInterfaces (Type type)
    {
      Debug.Assert (type.BaseType != null);
      // ReSharper disable PossibleNullReferenceException
      var interfaces = type.GetInterfaces().Except (type.BaseType.GetInterfaces()).Where (_validationTypeFilter.IsValidatableType).ToList();
      // ReSharper restore PossibleNullReferenceException
      if (!interfaces.Any())
        return Enumerable.Empty<IEnumerable<Type>>();

      var dependencies = new Dictionary<Type, IEnumerable<Type>>();
      foreach (var interfaceType in interfaces)
        dependencies[interfaceType] = interfaceType.GetInterfaces().ToList();

      return interfaces.TopologySortDesc (t => dependencies[t], _hierarchyLevelsubSort);
    }
  }
}