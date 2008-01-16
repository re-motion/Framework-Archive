using System;

namespace Rubicon.Mixins.Validation.Rules
{
  public interface IRuleSet
  {
    void Install (ValidatingVisitor visitor);
  }
}
