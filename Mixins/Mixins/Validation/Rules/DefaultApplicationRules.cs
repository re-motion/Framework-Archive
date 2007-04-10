using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.Validation.Rules
{
  public class DefaultApplicationRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.ApplicationRules.Add(new DelegateValidationRule<ApplicationDefinition> (ApplicationShouldContainAtLeastOneBaseClass));
    }

    public void ApplicationShouldContainAtLeastOneBaseClass (DelegateValidationRule<ApplicationDefinition>.Args args)
    {
      SingleShould (args.Definition.BaseClasses.GetEnumerator ().MoveNext (), args.Log, args.Self);
    }
  }
}
