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
using FluentValidation.Validators;
using System.Linq;
using Remotion.Validation.MetaValidation;
using FluentValidation;

namespace Remotion.Validation.UnitTests.IntegrationTests.TestDomain.MetaValidation
{
  public class MetaValidationTestClassCollector4 : ComponentValidationCollector<MetaValidationTestClass4>
  {
    public MetaValidationTestClassCollector4 ()
    {
      AddRule (c => c.Property1).AddMetaValidationRule<LengthValidator> (lengthRules =>
      {
        if (lengthRules.Count() > 1)
          return MetaValidationRuleValidationResult.CreateInvalidResult ("Invalid length validator count!");

        return MetaValidationRuleValidationResult.CreateValidResult();
      });

      AddRule (c => c.Property1).Length (4, 10);
      AddRule (c => c.Property1).Length (6, 9);
    }
  }
}