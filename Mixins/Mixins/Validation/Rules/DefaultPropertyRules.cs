using System;
using Mixins.Definitions;
using System.Reflection;
using Mixins.Validation;

namespace Mixins.Validation.Rules
{
  public class DefaultPropertyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.PropertyRules.Add (new DelegateValidationRule<PropertyDefinition> (NewMemberAddedByOverride));
    }

    private void NewMemberAddedByOverride (DelegateValidationRule<PropertyDefinition>.Args args)
    {
      SingleShould (args.Definition.Base != null ? (args.Definition.GetMethod == null || args.Definition.GetMethod.Base != null)
          && (args.Definition.SetMethod == null || args.Definition.SetMethod.Base != null)
          : true,
          args.Log,
          args.Self);
    }
  }
}
