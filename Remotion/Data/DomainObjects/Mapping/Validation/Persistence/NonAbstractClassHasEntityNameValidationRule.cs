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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  /// <summary>
  /// Validates that each non-abstract class in the mapping can resolve it's entity-name.
  /// </summary>
  public class NonAbstractClassHasEntityNameValidationRule : IPersistenceMappingValidationRule
  {
    public NonAbstractClassHasEntityNameValidationRule ()
    {
      
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.IsClassTypeResolved && classDefinition.StorageEntityDefinition is UnionViewDefinition && !classDefinition.IsAbstract)
      {
        yield return MappingValidationResult.CreateInvalidResultForType (
            classDefinition.ClassType,
            "Neither class '{0}' nor its base classes are mapped to a table. "
            + "Make class '{0}' abstract or define a table for it or one of its base classes.",
            classDefinition.ClassType.Name);
      }
      else
      {
        yield return MappingValidationResult.CreateValidResult ();
      }
    }
  }
}