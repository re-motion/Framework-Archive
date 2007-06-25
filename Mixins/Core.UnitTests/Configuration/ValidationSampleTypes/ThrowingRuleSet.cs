using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using Rubicon.Mixins.Validation.Rules;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ThrowingRuleSet : IRuleSet
  {
    public void Install (ValidatingVisitor visitor)
    {
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (delegate { throw new InvalidOperationException (); }));
    }
  }
}