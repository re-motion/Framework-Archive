using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.Validation.Rules
{
  public interface IRuleSet
  {
    void Install (ValidatingVisitor visitor);
  }
}
