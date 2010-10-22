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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the property mapping attributes are applied at the original property declaration.
  /// </summary>
  public class MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule : IClassDefinitionValidatorRule
  {
    public MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var errorMessages = new StringBuilder();
      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        var validationResult = Validate (propertyDefinition.PropertyInfo);
        if (!validationResult.IsValid)
          errorMessages.AppendLine (validationResult.Message);
      }

      var messages = errorMessages.ToString().Trim();
      return string.IsNullOrEmpty (messages) ? new MappingValidationResult (true) : new MappingValidationResult (false, messages);
    }

    private MappingValidationResult Validate (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!Utilities.ReflectionUtility.IsOriginalDeclaration (propertyInfo))
      {
        IMappingAttribute[] mappingAttributes = AttributeUtility.GetCustomAttributes<IMappingAttribute> (propertyInfo, false);
        if (mappingAttributes.Length > 0)
        {
          var message =
              string.Format (
                  "The '{0}' is a mapping attribute and may only be applied at the property's base definition.\r\n  Type: {1}, property: {2}",
                  mappingAttributes[0].GetType().FullName,
                  propertyInfo.DeclaringType.FullName,
                  propertyInfo.Name);
          return new MappingValidationResult (false, message);
        }
      }
      return new MappingValidationResult (true);
    }
  }
}