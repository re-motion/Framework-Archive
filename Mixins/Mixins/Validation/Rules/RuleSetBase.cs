using System;

namespace Mixins.Validation.Rules
{
  public abstract class RuleSetBase : IRuleSet
  {
    public abstract void Install (ValidatingVisitor visitor);

    protected void SingleShould (bool test, IValidationLog log, IValidationRule rule)
    {
      if (!test)
      {
        log.Warn (rule);
      }
      else
      {
        log.Succeed (rule);
      }
    }

    protected void SingleMust (bool test, IValidationLog log, IValidationRule rule)
    {
      if (!test)
      {
        log.Fail (rule);
      }
      else
      {
        log.Succeed (rule);
      }
    }
  }
}
