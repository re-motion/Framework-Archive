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
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Uses the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/> to build <see cref="LengthValidator"/> 
  /// and <see cref="NotNullValidator"/> for properties.
  /// </summary>
  public class DomainObjectAttributesBasedValidationCollectorProvider : AttributeBasedValidationCollectorProviderBase
  {
    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      
      return
          types.SelectMany (t => t.GetProperties (PropertyBindingFlags | BindingFlags.DeclaredOnly))
              .Select (p => (IAttributesBasedValidationPropertyRuleReflector) new DomainObjectAttributesBasedValidationPropertyRuleReflector (p))
              .ToLookup (c => c.ValidatedProperty.DeclaringType);
    }
  }
}