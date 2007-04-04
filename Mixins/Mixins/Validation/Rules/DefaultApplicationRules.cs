using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultApplicationRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.ApplicationRules.Add(new DelegateValidationRule<ApplicationDefinition> (ApplicationShouldContainAtLeastOneBaseClass));
    }

    public void ApplicationShouldContainAtLeastOneBaseClass (ApplicationDefinition definition, IValidationLog log, DelegateValidationRule<ApplicationDefinition> self)
    {
      SingleShould (definition.BaseClasses.GetEnumerator ().MoveNext (), log, self);
    }
  }
}
