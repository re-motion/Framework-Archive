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
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Merging
{
  public class OrderPrecedenceValidationCollectorMerger : ValidationCollectorMergerBase
  {
    private readonly IPropertyValidatorExtractorFactory _propertyValidatorExtractorFactory;

    public OrderPrecedenceValidationCollectorMerger (IPropertyValidatorExtractorFactory propertyValidatorExtractorFactory)
    {
      _propertyValidatorExtractorFactory = propertyValidatorExtractorFactory;
    }

    protected override void MergeRules (IEnumerable<ValidationCollectorInfo> collectorGroup, List<IAddingComponentPropertyRule> collectedRules)
    {
      var collectorInfos = collectorGroup.ToArray();

      //first: remove registered validation types for all collectors in same group (provider is responsible for order of collectors!)
      if (collectedRules.Any())
      {
        var registrationsWithContext = GetValidatorRegistrationsWithContext(collectorInfos);
        var propertyValidatorExtractor = _propertyValidatorExtractorFactory.Create (registrationsWithContext, LogContext);
        foreach (var validationRule in collectedRules)
          validationRule.ApplyRemoveValidatorRegistrations (propertyValidatorExtractor);
      }

      //second: add new rules
      collectedRules.AddRange (collectorInfos.SelectMany (g => g.Collector.AddedPropertyRules));
    }

    protected override ILogContext CreateNewLogContext ()
    {
      return new DefaultLogContext ();
    }

    private IEnumerable<ValidatorRegistrationWithContext> GetValidatorRegistrationsWithContext (ValidationCollectorInfo[] collectorInfos)
    {
      return collectorInfos.Select (ci => ci.Collector)
                           .SelectMany (c => c.RemovedPropertyRules)
                           .SelectMany (r => r.Validators, RuleWithValidatorRegistrationInfo.Create)
                           .Select (
                               vi =>
                               new ValidatorRegistrationWithContext (vi.ValidatorRegistration, vi.RemovingComponentPropertyRule));
    }

    
    private struct RuleWithValidatorRegistrationInfo
    {
      private readonly IRemovingComponentPropertyRule _removingComponentPropertyRule;
      private readonly ValidatorRegistration _validatorRegistration;

      public static RuleWithValidatorRegistrationInfo Create (
          IRemovingComponentPropertyRule removingComponentPropertyRule, ValidatorRegistration validatorRegistration)
      {
        return new RuleWithValidatorRegistrationInfo (removingComponentPropertyRule, validatorRegistration);
      }

      private RuleWithValidatorRegistrationInfo (IRemovingComponentPropertyRule removingComponentPropertyRule, ValidatorRegistration validatorRegistration)
      {
        ArgumentUtility.CheckNotNull ("removingComponentPropertyRule", removingComponentPropertyRule);
        ArgumentUtility.CheckNotNull ("validatorRegistration", validatorRegistration);

        _removingComponentPropertyRule = removingComponentPropertyRule;
        _validatorRegistration = validatorRegistration;
      }

      public IRemovingComponentPropertyRule RemovingComponentPropertyRule
      {
        get { return _removingComponentPropertyRule; }
      }

      public ValidatorRegistration ValidatorRegistration
      {
        get { return _validatorRegistration; }
      }
    }
  }
}