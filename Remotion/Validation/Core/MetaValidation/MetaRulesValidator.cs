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
using FluentValidation;
using FluentValidation.Internal;
using Remotion.Utilities;
using Remotion.Validation.Rules;

namespace Remotion.Validation.MetaValidation
{
  public class MetaRulesValidator : IMetaRuleValidator
  {
    private readonly IAddingComponentPropertyMetaValidationRule[] _addedPropertyMetaValidationRules;
    private readonly Func<MemberInfo, ISystemMetaValidationRulesProvider> _systemMetaValidationRulesProviderFactory;

    public MetaRulesValidator (
        IAddingComponentPropertyMetaValidationRule[] addingComponentPropertyMetaValidationRules,
        Func<MemberInfo, ISystemMetaValidationRulesProvider> systemMetaValidationRulesProviderFactory)
    {
      ArgumentUtility.CheckNotNull ("addingComponentPropertyMetaValidationRules", addingComponentPropertyMetaValidationRules);
      ArgumentUtility.CheckNotNull ("systemMetaValidationRulesProviderFactory", systemMetaValidationRulesProviderFactory);

      _addedPropertyMetaValidationRules = addingComponentPropertyMetaValidationRules;
      _systemMetaValidationRulesProviderFactory = systemMetaValidationRulesProviderFactory;
    }

    public IEnumerable<MetaValidationRuleValidationResult> Validate (IValidationRule[] validationRules)
    {
      ArgumentUtility.CheckNotNull ("validationRules", validationRules);

      //DelegateValidators (e.g. Conditions) are filtered!
      var propertyRulesByMemberInfo = validationRules.OfType<PropertyRule>().ToLookup (pr => pr.Member, pr => pr.Validators);

      return from propertyRuleGroup in _addedPropertyMetaValidationRules.ToLookup (pr => pr.Property)
             let metaValidationRules = GetAllMetaValidationRules (propertyRuleGroup)
             from metaValidationRule in metaValidationRules
                 .SelectMany (mvr => mvr.Validate (propertyRulesByMemberInfo[propertyRuleGroup.Key].SelectMany (v => v)))
             select metaValidationRule;
    }

    private IEnumerable<IMetaValidationRule> GetAllMetaValidationRules (IGrouping<MemberInfo, IAddingComponentPropertyMetaValidationRule> propertyRuleGroup)
    {
      return
          _systemMetaValidationRulesProviderFactory (propertyRuleGroup.Key)
              .GetSystemMetaValidationRules()
              .Concat (propertyRuleGroup.SelectMany (pr => pr.MetaValidationRules));
    }
  }
}