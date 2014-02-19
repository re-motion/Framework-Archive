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
using FluentValidation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  //TODO AO: doc
  public class CompoundValidationRuleMetadataService : IValidationRuleMetadataService
  {
    private readonly IValidationRuleMetadataService[] _validationRuleGlobalizationServices;

    public CompoundValidationRuleMetadataService (IEnumerable<IValidationRuleMetadataService> validationRuleGlobalizationServices)
    {
      ArgumentUtility.CheckNotNull ("validationRuleGlobalizationServices", validationRuleGlobalizationServices);

      _validationRuleGlobalizationServices = validationRuleGlobalizationServices.ToArray();
    }

    public IValidationRuleMetadataService[] ValidationRuleGlobalizationServices
    {
      get { return _validationRuleGlobalizationServices; }
    }

    public void ApplyMetadata (IValidationRule validationRule, Type typeToValidate)
    {
      ArgumentUtility.CheckNotNull ("validationRule", validationRule);
      ArgumentUtility.CheckNotNull ("validationRule", validationRule);

      foreach (var validatorGlobalizationService in _validationRuleGlobalizationServices)
        validatorGlobalizationService.ApplyMetadata (validationRule, typeToValidate);
    }
  }
}