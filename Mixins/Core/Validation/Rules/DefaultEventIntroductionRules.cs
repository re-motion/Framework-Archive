using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultEventIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      // currently no default rules
    }
  }
}