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
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that a property type is supported.
  /// </summary>
  public class PropertyTypeIsSupportedValidationRule : IPropertyDefinitionValidationRule
  {
    public PropertyTypeIsSupportedValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return from PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions
             select Validate (propertyDefinition.PropertyInfo, classDefinition);
    }

    private MappingValidationResult Validate (PropertyInfo propertyInfo, ClassDefinition classDefinition)
    {
      var nativePropertyType = ReflectionUtility.IsDomainObject (propertyInfo.PropertyType) ? typeof (ObjectID) : propertyInfo.PropertyType;
      if (!PropertyValue.IsTypeSupported (nativePropertyType))
      {
        return MappingValidationResult.CreateInvalidResult (
            propertyInfo,
            "The property type '{0}' is not supported. If you meant to declare a relation, '{0}' must be derived from '{1}'.\r\n\r\n"
            + "Declaring type: '{2}'\r\nProperty: '{3}'",
            nativePropertyType.Name,
            typeof (DomainObject).Name,
            propertyInfo.DeclaringType.FullName,
            propertyInfo.Name);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}