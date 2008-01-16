using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using Rubicon.Mixins.Validation.Rules;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
{
  public class ThrowingRuleSet : IRuleSet
  {
    public void Install (ValidatingVisitor visitor)
    {
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (delegate { throw new InvalidOperationException (); }));
    }
  }
}