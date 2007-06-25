using System;
using Mixins.Definitions;
using Mixins.Validation;
using Mixins.Validation.Rules;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ThrowingRuleSet : IRuleSet
  {
    public void Install (ValidatingVisitor visitor)
    {
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (delegate { throw new InvalidOperationException (); }));
    }
  }
}