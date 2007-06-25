using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.Validation;

namespace Mixins.Validation.Rules
{
  public class DefaultPropertyIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      // currently no default rules
    }
  }
}