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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  public class RelationEndPointPropertyTypeIsSupportedValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
          return new MappingValidationResult (false, validationResult.Message);
      }

      return new MappingValidationResult (true);
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      var relationEndPointAsReflectionBasedVirtualRelationEndPoint = relationEndPointDefinition as ReflectionBasedVirtualRelationEndPointDefinition;
      if (relationEndPointAsReflectionBasedVirtualRelationEndPoint != null)
      {
        var propertyInfo = relationEndPointAsReflectionBasedVirtualRelationEndPoint.PropertyInfo;
        var relationAttribute = (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (propertyInfo, typeof (BidirectionalRelationAttribute), true);
        if (relationAttribute == null && !typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType))
        {
          var message = string.Format ("The property type of an uni-directional relation property must be assignable to {0}.", typeof (DomainObject));
          return new MappingValidationResult (false, message);
        }
      }

      return new MappingValidationResult (true);
    }
  }
}